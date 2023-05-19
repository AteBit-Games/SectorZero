using Runtime.Navigation;
using UnityEngine;

namespace Runtime.BehaviourTree 
{
    public class Context
    {
        public Transform transform;
        public Animator animator;
        public NavAgent agent;
        public BehaviourTree tree;

        public static Context CreateFromGameObject(GameObject gameObject) 
        {
            Context context = new Context
            {
                transform = gameObject.transform,
                animator = gameObject.GetComponent<Animator>(),
                agent = gameObject.GetComponentInChildren<NavAgent>(),
                tree = gameObject.GetComponent<BehaviourTreeOwner>().behaviourTree
            };

            return context;
        }
    }
}