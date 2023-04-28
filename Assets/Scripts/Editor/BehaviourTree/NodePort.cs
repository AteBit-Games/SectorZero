using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.BehaviourTree 
{
    public class NodePort : Port 
    {
        private class DefaultEdgeConnectorListener : IEdgeConnectorListener
        {
            private readonly GraphViewChange m_GraphViewChange;
            private readonly List<Edge> m_EdgesToCreate;
            private readonly List<GraphElement> m_EdgesToDelete;

            public DefaultEdgeConnectorListener() 
            {
                m_EdgesToCreate = new List<Edge>();
                m_EdgesToDelete = new List<GraphElement>();
                m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
            }

            public void OnDropOutsidePort(Edge edge, Vector2 position) 
            {
                NodeView nodeSource = null;
                bool isSourceParent = false;
                
                if (edge.output != null) 
                {
                    nodeSource = edge.output.node as NodeView;
                    isSourceParent = true;
                }
                
                if (edge.input != null) 
                {
                    nodeSource = edge.input.node as NodeView;
                    isSourceParent = false;
                }
                CreateNodeWindow.Show(position, nodeSource, isSourceParent);
            }

            public void OnDrop(GraphView graphView, Edge edge) 
            {
                m_EdgesToCreate.Clear();
                m_EdgesToCreate.Add(edge);
                
                m_EdgesToDelete.Clear();
                if (edge.input.capacity == Capacity.Single)
                    foreach (Edge edgeToDelete in edge.input.connections)
                        if (edgeToDelete != edge)
                            m_EdgesToDelete.Add(edgeToDelete);
                if (edge.output.capacity == Capacity.Single)
                    foreach (Edge edgeToDelete in edge.output.connections)
                        if (edgeToDelete != edge)
                            m_EdgesToDelete.Add(edgeToDelete);
                if (m_EdgesToDelete.Count > 0) graphView.DeleteElements(m_EdgesToDelete);

                var edgesToCreate = m_EdgesToCreate;
                if (graphView.graphViewChanged != null) 
                {
                    edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
                }

                foreach (Edge e in edgesToCreate) 
                {
                    graphView.AddElement(e);
                    edge.input.Connect(e);
                    edge.output.Connect(e);
                }
            }
        }

        public NodePort(Direction direction, Capacity capacity) : base(Orientation.Vertical, direction, capacity, typeof(bool)) 
        {
            var connectorListener = new DefaultEdgeConnectorListener();
            m_EdgeConnector = new EdgeConnector<Edge>(connectorListener);
            this.AddManipulator(m_EdgeConnector);
            style.width = direction == Direction.Input ? 100 : 40;

            visualClass = "node-port";
            SetupStyle(direction);

            portColor = new Color(0.36f, 0.96f, 1f);
        }

        private void SetupStyle(Direction direction)
        {
            switch (direction)
            {
                case Direction.Input:
                    style.width = Length.Percent(40);
                    style.height = 10;
                    style.borderBottomLeftRadius = 3;
                    style.borderBottomRightRadius = 3;
                    
                    
                    var inputConnector = this.Q("connector");
                    inputConnector.style.borderBottomWidth = 0;
                    inputConnector.style.borderTopWidth = 0;
                    inputConnector.style.borderLeftWidth = 0;
                    inputConnector.style.borderRightWidth = 0;
                    
                    var inputConnectorText = this.Q("type");
                    inputConnectorText.style.height = 2;

                    break;
                case Direction.Output:
                    style.width = Length.Percent(80);
                    style.height = 10;
                    style.borderTopLeftRadius = 3;
                    style.borderTopRightRadius = 3;
                    
                    
                    var outputConnector = this.Q("connector");
                    outputConnector.style.borderBottomWidth = 0;
                    outputConnector.style.borderTopWidth = 0;
                    outputConnector.style.borderLeftWidth = 0;
                    outputConnector.style.borderRightWidth = 0;
                    
                    var ouptutConnectorText = this.Q("type");
                    ouptutConnectorText.style.height = 2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public override bool ContainsPoint(Vector2 localPoint) 
        {
            Rect rect = new Rect(0, 0, layout.width, layout.height);
            return rect.Contains(localPoint);
        }
    }
}