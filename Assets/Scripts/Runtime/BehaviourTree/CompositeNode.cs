using System.Collections.Generic;
using UnityEngine;

namespace Runtime.BehaviourTree
{
    [System.Serializable]
    public abstract class CompositeNode : Node
    {
        [HideInInspector] [SerializeReference] public List<Node> children = new();
        
        
        public int GetChildIndex(Node node)
        {
            return children.IndexOf(node);
        }
    }
}