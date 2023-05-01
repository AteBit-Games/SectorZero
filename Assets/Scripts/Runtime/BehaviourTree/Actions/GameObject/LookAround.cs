using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.GameObject 
{
    [Serializable]
    public class LookAround : ActionNode
    {
        [Space(10)]
        [Header("GENERAL PROPERTIES")]
        [Tooltip("The layer mask of the target.")]
        public NodeProperty<LayerMask> targetMask = new(){Value = -1};
        [Tooltip("The layer mask of obstacles.")]
        public NodeProperty<LayerMask> obstacleMask = new(){Value = -1};

        [Space(10)]
        [Header("SIGHT PROPERTIES")]
        public NodeProperty<float> maxDistance = new(){Value = 50f};
        [Tooltip("The view angle to use for the check.")]
        public NodeProperty<float> viewAngle = new(){Value = 70f};
        [Tooltip("Duration of the look around.")]
        public float duration = 5f;
        
        [Space(10)]
        [Header("OUTPUT PROPERTIES")]
        [Tooltip("The direction the agent is looking at.")]
        public NodeProperty<Vector2> lookDirection;
        
        private float _startTime;

        protected override void OnStart()
        {
            _startTime = Time.time;
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            // Let the agent look around for a while.
            // if(Time.time - _startTime < duration)
            // {
            //     return State.Running;
            // }
            //
            //
            //
            //
            //
            //
            
            return State.Running;
        }
        
        protected override void OnReset() { }
        
        public override void OnDrawGizmos()
        {
            if(context != null && drawGizmos)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(context.transform.position, maxDistance.Value);
                
                // var halfFOV = viewAngle.Value / 2.0f;
                // Vector3 direction = GetDirection() == Vector2.zero ? Vector2.right : GetDirection();
                // var beginDirection = Quaternion.AngleAxis(-halfFOV, Vector3.forward) * direction;
                // var endDirection = Quaternion.AngleAxis(halfFOV, Vector3.forward) * direction;
                //
                // Gizmos.color = Color.red;
                // Gizmos.DrawLine(context.transform.position, context.transform.position + beginDirection * maxDistance.Value);
                // Gizmos.DrawLine(context.transform.position, context.transform.position + endDirection * maxDistance.Value);
            }
        }
    }
}
