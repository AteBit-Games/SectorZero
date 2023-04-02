// Copyright (c) 2023 AteBit Games

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Runtime.SoundSystem.Propagation
{
    [ExecuteInEditMode]
    public class AudioPropagationNodeGroup : MonoBehaviour
    {
        private static SoundNode[] _allNodes;
        [HideInInspector] public float maxConnectDistance = 10f;
        [HideInInspector] public int nodeCount;
        
        public List<SoundNode> nodes;
        public GameObject soundNodePrefab;
        private int _nextNodeID;

        public static SoundNode[] AllNodes => _allNodes ??= GetAllNodes();

        public static SoundNode[] GetAllNodes()
        {
            var nodeGroups = Object.FindObjectsOfType<AudioPropagationNodeGroup>();
            int nodesSum = nodeGroups.Sum(audioPropagateNodeGroup => audioPropagateNodeGroup.nodeCount);

            var nodes = new SoundNode[nodesSum];
            int index = 0;
            foreach (var audioPropagateNodeGroup in nodeGroups)
            {
                for (int k = 0; k < audioPropagateNodeGroup.nodeCount; k++)
                {
                    nodes[index++] = audioPropagateNodeGroup.nodes[k];
                }
            }

            return nodes;
        }

        public void AddNode(Vector3 position)
        {
            var newNodeObject = Instantiate(soundNodePrefab, position, Quaternion.identity, transform);
            var node = newNodeObject.AddComponent<SoundNode>();
            
            node.id = _nextNodeID++;
            node.connections = new List<SoundNode>();
            ConnectToNearbyNodes(node);
            nodes.Add(node);
            nodeCount++;
        }

        private void ConnectToNearbyNodes(SoundNode node)
        {
            foreach (var connectionNode in nodes)
            {
                if (connectionNode.id != node.id)
                {
                    float num = Vector3.Distance(node.gameObject.transform.position, connectionNode.gameObject.transform.position);
                    if (num < maxConnectDistance && !Physics.Linecast(node.gameObject.transform.position, connectionNode.gameObject.transform.position))
                    {
                        ConnectNodes(node, connectionNode);
                    }
                }
            }
        }

        public void DeleteNode(SoundNode node)
        {
            DisconnectAll(node);
            nodes.Remove(node);
            nodeCount--;

            foreach (var otherNode in nodes)
            {
                RefreshNodeConnections(otherNode);
            }
        }

        public void DeleteAllNodes()
        {
            nodes.Clear();
            nodeCount = 0;
        }

        public void RefreshNodeConnections(SoundNode node)
        {
            DisconnectAll(node);
            ConnectToNearbyNodes(node);
        }

        public SoundNode FindNodeByID(int id)
        {
            foreach (var node in nodes)
            {
                if (node.id == id)
                {
                    return node;
                }
            }

            return null;
        }

        public static void ConnectNodes(SoundNode n1, SoundNode n2)
        {
            if (n1.id != n2.id && !n1.connections.Contains(n2) && !n2.connections.Contains(n1))
            {
                n1.connections.Add(n2);
                n2.connections.Add(n1);
            }
        }

        private void DisconnectNodes(SoundNode n1, SoundNode n2)
        {
            n1.connections.Remove(n2);
            n2.connections.Remove(n1);
        }

        private void DisconnectAll(SoundNode node)
        {
            if (node.connections.Count > 0)
            {
                int count = node.connections.Count;
                for (int num = count - 1; num >= 0; num--)
                {
                    SoundNode n = node.connections[num];
                    DisconnectNodes(node, n);
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (nodes == null)
            {
                return;
            }

            if (IsSelected())
            { 
                foreach (var node in nodes)
                {
                    foreach (var connection in node.connections)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawLine(node.transform.position, connection.transform.position);
                    }
                }
            }
        }

        private bool IsSelected()
        {
            return Selection.transforms.Contains(transform) || nodes.Any(node => Selection.transforms.Contains(node.transform));
        }
    }
#endif
}
