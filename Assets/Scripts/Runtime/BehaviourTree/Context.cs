using UnityEngine;
using UnityEngine.AI;

namespace Runtime.BehaviourTree 
{
    public class Context
    {
        public Transform transform;
        public Animator animator;
        public NavMeshAgent agent;
        public BehaviourTree tree;

        public static Context CreateFromGameObject(GameObject gameObject) 
        {
            Context context = new Context
            {
                transform = gameObject.transform,
                animator = gameObject.GetComponent<Animator>(),
                agent = gameObject.transform.parent.gameObject.GetComponentInChildren<NavMeshAgent>(),
                tree = gameObject.GetComponent<BehaviourTreeOwner>().behaviourTree
            };

            return context;
        }
    }
}