using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Conditions.GameObject
{
    [Serializable]
    [Name("Detect Target")]
    [Category("GameObject")]
    [Description("Validates if the agent can detect a target using line of sight and awareness.")]
    public class Hear : ConditionNode
    {
        [Space(10)]
        [Header("GENERAL PROPERTIES")]
        public NodeProperty<UnityEngine.GameObject> target;
        [Tooltip("Whether the target is hiding inside an object.")]
        public NodeProperty<bool> isTargetHiding;
        [Tooltip("The layer mask of the target.")]
        public NodeProperty<LayerMask> targetMask = new(){Value = -1};
        [Tooltip("The layer mask of obstacles.")]
        public NodeProperty<LayerMask> obstacleMask = new(){Value = -1};

        [Space(10)]
        [Header("SIGHT")]
        [Tooltip("Enable sight check.")]
        public bool checkSight = true;
        [Tooltip("Distance within which to look.")]
        public NodeProperty<float> maxDistance = new(){Value = 50f};
        [Tooltip("The view angle to use for the check.")]
        public NodeProperty<float> viewAngle = new(){Value = 70f};
        
        [Space(10)]
        [Header("AWARENESS")]
        [Tooltip("Enable awareness check.")]
        public bool checkAwareness = true;
        [Tooltip("Distance within which the target can be seen (or rather sensed) regardless of view angle.")]
        public NodeProperty<float> awarenessDistance = new(){Value = 0f};
        
        [Space(10)]
        [Header("DEBUG")]
        [Tooltip("Enable debug messages.")]
        public bool showDebugMessages;

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (isTargetHiding.Value) return State.Failure;
            
            if (checkAwareness)
            {
                var targetsInAwarenessRadius = Physics2D.OverlapCircleAll(context.transform.position, awarenessDistance.Value, targetMask.Value);
                foreach (var currentTarget in targetsInAwarenessRadius)
                {
                    var directionToTarget = (currentTarget.transform.position - context.transform.position).normalized;
                    float distanceToTarget = Vector2.Distance(context.transform.position, currentTarget.transform.position);
                    if (!Physics2D.Raycast(context.transform.position, directionToTarget, distanceToTarget, obstacleMask.Value))
                    {
                        if (showDebugMessages) Debug.Log($"CanSeeTarget: Target is sensed by monster");
                        return State.Success;
                    }
                    
                    if (showDebugMessages) Debug.Log($"CanSeeTarget: Target is sensed by monster but is behind an obstacle");
                }
            }

            if (checkSight)
            {
                var targetTransform = target.Value.transform;
                // // Check if the target is within the max distance
                if(Vector2.Distance(context.transform.position, targetTransform.position) > maxDistance.Value)
                {
                    return State.Failure;
                }
                
                // Check if the target is within the view angle
                if(Vector2.Angle(GetDirection(), targetTransform.position - context.transform.position) > viewAngle.Value)
                {
                    return State.Failure;
                }
                
                var targetsInViewRadius = Physics2D.OverlapCircleAll(context.transform.position, maxDistance.Value, targetMask.Value);
                foreach (var currentTarget in targetsInViewRadius)
                {
                    var directionToTarget = (currentTarget.transform.position - context.transform.position).normalized;
                    Vector3 direction = GetDirection() == Vector2.zero ? Vector2.right : GetDirection();
                    if (Vector2.Angle(direction, directionToTarget) < viewAngle.Value / 2)
                    {
                        float distanceToTarget = Vector2.Distance(context.transform.position, currentTarget.transform.position);
                        if (!Physics2D.Raycast(context.transform.position, directionToTarget, distanceToTarget, obstacleMask.Value))
                        {
                            if(showDebugMessages) Debug.Log($"CanSeeTarget: Target can be seen.");
                            return State.Success;
                        }
                        
                        if(showDebugMessages) Debug.Log($"CanSeeTarget: Target is within view angle but there is an obstacle in the way.");
                    }
                }
            }
            
            return State.Failure;
        }

        protected override void OnReset() { }

        public override void OnDrawGizmos()
        {
            if(context != null && drawGizmos)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(context.transform.position, maxDistance.Value);
                    
                var halfFOV = viewAngle.Value / 2.0f;
                Vector3 direction = GetDirection() == Vector2.zero ? Vector2.right : GetDirection();
                var beginDirection = Quaternion.AngleAxis(-halfFOV, Vector3.forward) * direction;
                var endDirection = Quaternion.AngleAxis(halfFOV, Vector3.forward) * direction;
                
                Gizmos.color = Color.red;
                Gizmos.DrawLine(context.transform.position, context.transform.position + beginDirection * maxDistance.Value);
                Gizmos.DrawLine(context.transform.position, context.transform.position + endDirection * maxDistance.Value);
            }
        }

        private Vector2 GetDirection()
        {
            return context.agent.MovingDirection;
        }
    }
}