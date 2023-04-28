using System.Collections.Generic;
using UnityEngine;

namespace Runtime.BehaviourTree 
{
    [CreateAssetMenu(fileName = "BehaviourTree", menuName = "Behaviour Tree/New Behaviour Tree")]
    public class BehaviourTree : ScriptableObject 
    {
        [SerializeReference]
        public RootNode rootNode;

        [SerializeReference]
        public List<Node> nodes = new();

        public Node.State treeState = Node.State.Running;

        public Blackboard blackboard = new();

        #region  EditorProperties 
        public Vector3 viewPosition = new(600, 300);
        public Vector3 viewScale = Vector3.one;
        #endregion

        public BehaviourTree() 
        {
            rootNode = new RootNode();
            nodes.Add(rootNode);
        }

        private void OnEnable() 
        {
            nodes.RemoveAll(node => node == null);
            Traverse(rootNode, node => {
                if (node is CompositeNode composite)
                {
                    composite.children.RemoveAll(child => child == null);
                }
            });
        }

        public Node.State Update() 
        {
            if (treeState == Node.State.Running) treeState = rootNode.Update();
            return treeState;
        }

        public static List<Node> GetChildren(Node parent) 
        {
            var children = new List<Node>();

            switch (parent)
            {
                case DecoratorNode { child: { } } decorator:
                    children.Add(decorator.child);
                    break;
                case RootNode { child: { } } rootNode:
                    children.Add(rootNode.child);
                    break;
                case CompositeNode composite:
                    return composite.children;
            }

            return children;
        }

        public static void Traverse(Node node, System.Action<Node> visitor) 
        {
            if (node != null) 
            {
                visitor.Invoke(node);
                var children = GetChildren(node);
                children.ForEach((n) => Traverse(n, visitor));
            }
        }

        public BehaviourTree Clone() 
        {
            BehaviourTree tree = Instantiate(this);
            return tree;
        }

        public void Bind(Context context) 
        {
            Traverse(rootNode, node => {
                node.context = context;
                node.blackboard = blackboard;
                node.OnInit();
            });
        }
    }
}