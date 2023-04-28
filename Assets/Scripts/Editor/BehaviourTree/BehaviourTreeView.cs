using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.BehaviourTree;
using Runtime.BehaviourTree.Composites;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;
using Node = Runtime.BehaviourTree.Node;

namespace Editor.BehaviourTree
{
    public class BehaviourTreeView : GraphView 
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> { }

        public const int gridSnapSize = 20;
        public Action<NodeView> OnNodeSelected;
        protected override bool canCopySelection => true;
        protected override bool canCutSelection => false;
        protected override bool canPaste => true;
        protected override bool canDuplicateSelection => true;
        protected override bool canDeleteSelection => true;

        private SerializedBehaviourTree serializer;
        private bool dontUpdateModel;

        [Serializable]
        private class CopyPasteData {
            public List<string> nodeGuids = new();

            public void AddGraphElements(IEnumerable<GraphElement> elementsToCopy)
            {
                foreach (var element in elementsToCopy) 
                {
                    if (element is NodeView { node: not RootNode } nodeView) 
                    {
                        nodeGuids.Add(nodeView.node.guid);
                    }
                }
            }
        }

        private class EdgeToCreate 
        {
            public NodeView parent;
            public NodeView child;
        };

        public BehaviourTreeView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new HierarchySelector());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            serializeGraphElements = (items) => {
                CopyPasteData copyPasteData = new CopyPasteData();
                copyPasteData.AddGraphElements(items);
                string data = JsonUtility.ToJson(copyPasteData);
                return data;
            };
            
            unserializeAndPaste = (_, data) => {
                serializer.BeginBatch();
                ClearSelection();

                CopyPasteData copyPasteData = JsonUtility.FromJson<CopyPasteData>(data);
                var oldToNewMapping = new Dictionary<string, string>();
                
                var nodesToCopy = new List<NodeView>();
                foreach (var nodeGuid in copyPasteData.nodeGuids)
                {
                    NodeView nodeView = FindNodeView(nodeGuid);
                    nodesToCopy.Add(nodeView);
                }
                
                var edgesToCreate = new List<EdgeToCreate>();
                foreach (var nodeGuid in copyPasteData.nodeGuids) 
                {
                    NodeView nodeView = FindNodeView(nodeGuid);
                    var nodesParent = nodeView.NodeParent;
                    if (nodesToCopy.Contains(nodesParent)) 
                    {
                        EdgeToCreate newEdge = new EdgeToCreate
                        {
                            parent = nodesParent,
                            child = nodeView
                        };
                        edgesToCreate.Add(newEdge);
                    }
                }
                
                foreach (var nodeView in nodesToCopy) 
                {
                    Node newNode = serializer.CreateNode(nodeView.node.GetType(), nodeView.node.position + Vector2.one * 50);
                    NodeView newNodeView = CreateNodeView(newNode);
                    AddToSelection(newNodeView);
                    
                    oldToNewMapping[nodeView.node.guid] = newNode.guid;
                }
                
                foreach(var edge in edgesToCreate) 
                {
                    NodeView oldParent = edge.parent;
                    NodeView oldChild = edge.child;
                    
                    NodeView newParent = FindNodeView(oldToNewMapping[oldParent.node.guid]);
                    NodeView newChild = FindNodeView(oldToNewMapping[oldChild.node.guid]);

                    serializer.AddChild(newParent.node, newChild.node);
                    AddChild(newParent, newChild);
                }
                
                serializer.EndBatch();
            };
            
            canPasteSerializedData = _ => true;
            viewTransformChanged += OnViewTransformChanged;
        }

        private void OnViewTransformChanged(GraphView graphView) 
        {
            Vector3 position = contentViewContainer.transform.position;
            Vector3 transformScale = contentViewContainer.transform.scale;
            serializer.SetViewTransform(position, transformScale);
        }

        public NodeView FindNodeView(Node node) 
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }

        private NodeView FindNodeView(string guid) 
        {
            return GetNodeByGuid(guid) as NodeView;
        }

        public void ClearView() 
        {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged; 
        }

        public void PopulateView(SerializedBehaviourTree tree) 
        {
            serializer = tree;
            ClearView();
            Debug.Assert(serializer.tree.rootNode != null);
            serializer.tree.nodes.ForEach(n => CreateNodeView(n));
            
            serializer.tree.nodes.ForEach(n => {
                var children = Runtime.BehaviourTree.BehaviourTree.GetChildren(n);
                children.ForEach(c => {
                    NodeView parentView = FindNodeView(n);
                    NodeView childView = FindNodeView(c);
                    Debug.Assert(parentView != null, "Invalid parent after deserializing");
                    Debug.Assert(childView != null, $"Null child view after deserializing parent{parentView.node.GetType().Name}");
                    CreateEdgeView(parentView, childView);
                });
            });
            
            contentViewContainer.transform.position = serializer.tree.viewPosition;
            contentViewContainer.transform.scale = serializer.tree.viewScale;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) 
        {
            return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction &&
            endPort.node != startPort.node).ToList();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange) 
        {
            if (dontUpdateModel) return graphViewChange;
            var blockedDeletes = new List<GraphElement>();

            if (graphViewChange.elementsToRemove != null) 
            {
                graphViewChange.elementsToRemove.ForEach(elem => {
                    if (elem is NodeView nodeView) 
                    {
                        if (nodeView.node is not RootNode) 
                        {
                            OnNodeSelected(null);
                            serializer.DeleteNode(nodeView.node);
                            RemoveNodeView();
                        } 
                        else 
                        {
                            blockedDeletes.Add(elem);
                        }
                    }

                    if (elem is Edge edge) 
                    {
                        NodeView parentView = edge.output.node as NodeView;
                        NodeView childView = edge.input.node as NodeView;
                        serializer.RemoveChild(parentView.node, childView.node);
                    }
                });
            }

            graphViewChange.edgesToCreate?.ForEach(edge => {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                serializer.AddChild(parentView.node, childView.node);
            });

            nodes.ForEach((n) => {
                if (n is NodeView view)
                {
                    view.SetupDataBinding();
                    view.SortChildren();
                }
            });

            foreach (var elem in blockedDeletes.Where(_ => graphViewChange.elementsToRemove != null))
            {
                graphViewChange.elementsToRemove?.Remove(elem);
            }
            return graphViewChange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) 
        {
            CreateNodeWindow.Show(evt.mousePosition, null);
        }

        public NodeView CreateNode(Type type, Vector2 position, NodeView parentView) 
        {
            serializer.BeginBatch();
            
            Node node = serializer.CreateNode(type, position);
            if (parentView != null) serializer.AddChild(parentView.node, node);
            
            NodeView nodeView = CreateNodeView(node);
            if (parentView != null) AddChild(parentView, nodeView);

            serializer.EndBatch();
            return nodeView;
        }

        public NodeView CreateNodeWithChild(Type type, Vector2 position, NodeView childView)
        {
            serializer.BeginBatch();
            Node node = serializer.CreateNode(type, position);
            
            foreach(var connection in childView.input.connections)
            {
                if (connection.output.node is NodeView childParent) serializer.RemoveChild(childParent.node, childView.node);
            }
            serializer.AddChild(node, childView.node);
            
            NodeView nodeView = CreateNodeView(node);
            if (nodeView != null)  AddChild(nodeView, childView);

            serializer.EndBatch();
            return nodeView;
        }

        private NodeView CreateNodeView(Node node)
        {
            NodeView nodeView = new NodeView(node, BehaviourTreeEditorWindow.Instance.nodeXml);
            AddElement(nodeView);

            nodeView.OnNodeSelected = OnNodeSelected;
            return nodeView;
        }

        private void AddChild(NodeView parentView, NodeView childView) 
        {
            if (parentView.output.capacity == Port.Capacity.Single) RemoveElements(parentView.output.connections);
            RemoveElements(childView.input.connections);
            CreateEdgeView(parentView, childView);
        }

        private void CreateEdgeView(NodeView parentView, NodeView childView)
        {
            Edge edge = parentView.output.ConnectTo(childView.input);
            AddElement(edge);
        }

        private void RemoveElements(IEnumerable<GraphElement> elementsToRemove) 
        {
            dontUpdateModel = true;
            DeleteElements(elementsToRemove);
            dontUpdateModel = false;
        }

        public void UpdateNodeStates()
        {
            nodes.ForEach(n =>
            {
                NodeView view = n as NodeView;
                view?.UpdateState();
            });
        }
        
        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);
            if (evt.eventTypeId == MouseDownEvent.TypeId())
            {
                if (evt is MouseDownEvent { button: 0, clickCount: 1 })
                {
                    RemoveNodeView();
                }
            }
        }

        private static void RemoveNodeView()
        {
            var container = BehaviourTreeEditorWindow.Instance.rootVisualElement.Q("main-container");
            container.ClearClassList();
            container.AddToClassList("none-selected");
        }

        public void SelectNode(NodeView nodeView) 
        {
            ClearSelection();
            if (nodeView != null) AddToSelection(nodeView);
        }
    }
}