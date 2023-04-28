using Runtime.Navigation;
using UnityEngine;

namespace Runtime.BehaviourTree 
{
    public class Context
    {
        public GameObject gameObject;
        public Transform transform;
        public Animator animator;
        public Rigidbody2D physics;
        public Collider2D collider;
        public NavAgent agent;

        public static Context CreateFromGameObject(GameObject gameObject) 
        {
            Context context = new Context
            {
                gameObject = gameObject,
                transform = gameObject.transform,
                animator = gameObject.GetComponent<Animator>(),
                physics = gameObject.GetComponent<Rigidbody2D>(),
                collider = gameObject.GetComponent<Collider2D>(),
                agent = gameObject.GetComponent<NavAgent>()
            };

            return context;
        }
    }
}