using UnityEngine;
using UnityEngine.AI;

namespace Runtime.BehaviourTree 
{
    public class Context
    {
        public BehaviourTreeOwner owner;
        public Transform transform;
        public Animator animator;
        public NavMeshAgent agent;
        public BehaviourTree tree;

        public static Context CreateFromGameObject(GameObject gameObject, BehaviourTree tree) 
        {
            Context context = new Context
            {
                owner = gameObject.GetComponent<BehaviourTreeOwner>(),
                transform = gameObject.transform,
                animator = gameObject.GetComponent<Animator>(),
                agent = gameObject.transform.parent.gameObject.GetComponentInChildren<NavMeshAgent>(),
                tree = tree
            };

            return context;
        }
    }
}