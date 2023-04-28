using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.Navigation 
{
    [Serializable]
    [Name("Wander")]
    [Category("Navigation")]
    [Description("Wander around the map")]
    public class Wander : ActionNode
    {
        public NodeProperty<float> speed = new(){Value=4f};
        public NodeProperty<float> keepDistance = new(){Value=0.1f};
        public NodeProperty<float> minWanderDistance = new(){Value=5f};
        public NodeProperty<float> maxWanderDistance = new(){Value=20f};

        protected override void OnStart()
        {
            context.agent.maxSpeed = speed.Value;
            DoWander();
        }

        protected override void OnStop()
        {
            context.agent.Stop();
        }
    
        protected override State OnUpdate() 
        {
            var agent = context.agent;
            if(!agent.PathPending && agent.RemainingDistance <= agent.stoppingDistance + keepDistance.Value)
            {
                return State.Success;
            }
            
            return State.Running;
        }

        private void DoWander()
        {
            var min = minWanderDistance.Value;
            var max = maxWanderDistance.Value;
            
            min = Mathf.Clamp(min, 0.01f, max);
            max = Mathf.Clamp(max, min, max);
            
            var wanderPos = context.agent.Position;
            while((wanderPos - context.agent.Position).sqrMagnitude < min || !context.agent.Map.PointIsValid(wanderPos))
            {
                wanderPos = (UnityEngine.Random.insideUnitCircle * max) + context.agent.Position;
            }
            
            if(context.agent.Map.PointIsValid(wanderPos))
            {
                context.agent.SetDestination(wanderPos);
            }
        }
        protected override void OnReset()
        {
            context.agent.Stop();
        }
        
        public override void OnDrawGizmos()
        {
            if(!drawGizmos || context == null || context.agent == null) return;
            
            var min = minWanderDistance.Value;
            var max = maxWanderDistance.Value;
            
            min = Mathf.Clamp(min, 0.01f, max);
            max = Mathf.Clamp(max, min, max);
            
            // Draw the minimum distance
            Gizmos.color = new Color(0.19f, 0.87f, 1f);
            Gizmos.DrawWireSphere(context.agent.Position, min);

            // Draw the maximum distance
            Gizmos.color = new Color(0f, 0.4f, 1f);
            Gizmos.DrawWireSphere(context.agent.Position, max);
        }
    }
}
