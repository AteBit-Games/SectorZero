using System;
using System.Collections.Generic;
using System.Linq;
using PlasticPipe.PlasticProtocol.Messages;
using Runtime.BehaviourTree;
using Runtime.BehaviourTree.Composites;
using Runtime.BehaviourTree.Decorators;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Node = Runtime.BehaviourTree.Node;
using Random = System.Random;
using Repeat = Runtime.BehaviourTree.Decorators.Repeat;

namespace Editor.BehaviourTree
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node 
    {
        public Action<NodeView> onNodeSelected;
        public readonly Node node;
        public Port input;
        public Port output;

        public NodeView NodeParent 
        {
            get
            {
                using var iter = input.connections.GetEnumerator();
                iter.MoveNext();
                return iter.Current?.output.node as NodeView;
            }
        }

        public List<NodeView> NodeChildren 
        {
            get
            {
                // This is untested and may not work. Possibly output should be input.
                return output.connections.Select(edge => edge.output.node).OfType<NodeView>().ToList();
            }
        }

        public NodeView(Node node, VisualTreeAsset nodeXml) : base(AssetDatabase.GetAssetPath(nodeXml)) 
        {
            capabilities &= ~(Capabilities.Snappable); // Disable node snapping
            this.node = node;
            title = node is RootNode ? node.Name.ToUpper() : node.Name;
            
            viewDataKey = node.guid;

            style.left = node.position.x;
            style.top = node.position.y;

            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();
            SetupDataBinding();

            this.AddManipulator(new DoubleClickNode());
        }
        
        public void SetupDataBinding() 
        {
            SerializedBehaviourTree serializer = BehaviourTreeEditorWindow.instance.serializer;
            var nodeProp = serializer.FindNode(serializer.Nodes, node);
            var commentProp = nodeProp?.FindPropertyRelative("comment");
            if (commentProp != null) 
            {
                Label commentLabel = this.Q<Label>("comment");
                commentLabel.BindProperty(commentProp);
            }
        }

        private void SetupClasses()
        {
            switch (node)
            {
                case ActionNode:
                    AddToClassList("action");
                    break;
                case CompositeNode:
                    AddToClassList("composite");
                    AddToClassList(GetCompositeType());
                    break;
                case DecoratorNode:
                    AddToClassList("decorator");
                    AddToClassList(GetDecoratorType());
                    break;
                case ConditionNode:
                    AddToClassList("condition");
                    break;
                case SubTree:
                    AddToClassList("subTree");
                    break;
                case RootNode:
                    AddToClassList("root");
                    break;
            }
        }
        
        private string GetCompositeType()
        {
            return node switch
            {
                Parallel => "parallelNode",
                ProbabilitySelector => "probabilitySelectorNode",
                RandomSelector => "randomSelectorNode",
                Selector => "selectorNode",
                Sequencer => "sequencerNode",
                Switch => "switchNode",
                _ => "compositeNode"
            };
        }
        
        private string GetDecoratorType()
        {
            return node switch
            {
                Inverter => "inverterNode",
                Repeat => "repeatNode",
                Timeout => "timeoutNode",
                Cooldown => "cooldownNode",
                Remapper => "remapperNode",
                _ => "decoratorNode"
            };
        }

        private void CreateInputPorts()
        {
            switch (node)
            {
                case ActionNode: case CompositeNode: case DecoratorNode: case ConditionNode: case SubTree:
                    input = new NodePort(Direction.Input, Port.Capacity.Single);
                    break;
                case RootNode:
                    break;
            }

            if (input != null) 
            {
                input.portName = "";
                input.style.flexDirection = FlexDirection.Column;
                inputContainer.Add(input);
            }
        }

        private void CreateOutputPorts()
        {
            switch (node)
            {
                case ActionNode: case ConditionNode: case SubTree:
                    break;
                case CompositeNode:
                    output = new NodePort(Direction.Output, Port.Capacity.Multi);
                    break;
                case DecoratorNode:
                case RootNode:
                    output = new NodePort(Direction.Output, Port.Capacity.Single);
                    break;
            }

            if (output != null) 
            {
                output.portName = "";
                output.style.flexDirection = FlexDirection.ColumnReverse;
                outputContainer.Add(output);
            }
        }

        public override void SetPosition(Rect newPos) 
        {
            newPos.x = EditorUtility.RoundTo(newPos.x, BehaviourTreeView.GridSnapSize);
            newPos.y = EditorUtility.RoundTo(newPos.y, BehaviourTreeView.GridSnapSize);

            base.SetPosition(newPos);

            SerializedBehaviourTree serializer = BehaviourTreeEditorWindow.instance.serializer;
            Vector2 position = new Vector2(newPos.xMin, newPos.yMin);
            serializer.SetNodePosition(node, position);
        }

        public override void OnSelected() 
        {
            base.OnSelected();
            onNodeSelected?.Invoke(this);

            var container = BehaviourTreeEditorWindow.instance.rootVisualElement.Q("main-container");
            container.ClearClassList();
            container.AddToClassList("node-selected");
        }

        public void SortChildren() 
        {
            if (node is CompositeNode composite) composite.children.Sort(SortByHorizontalPosition);
        }

        private static int SortByHorizontalPosition(Node left, Node right) 
        {
            return left.position.x < right.position.x ? -1 : 1;
        }

        public void UpdateState() 
        {
            RemoveFromClassList("running");
            RemoveFromClassList("failure");
            RemoveFromClassList("success");

            if (Application.isPlaying) {
                switch (node.state) {
                    case Node.State.Running:
                        if (node.started) {
                            AddToClassList("running");
                        }
                        break;
                    case Node.State.Failure:
                        AddToClassList("failure");
                        break;
                    case Node.State.Success:
                        AddToClassList("success");
                        break;
                    case Node.State.Inactive:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}