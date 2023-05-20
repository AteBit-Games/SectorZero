using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.Navigation 
{
    [Serializable]
    [Name("Seek (Vector2)")]
    [Category("Navigation")]
    [Description("Seek a target position")]
    public class MoveToPosition : ActionNode
    {
        public NodeProperty<Vector2> targetPosition;
        public NodeProperty<float> speed = new(){Value = 4f};
        
        private Vector3? _lastRequest;

        protected override void OnStart()
        {
            context.agent.speed = speed.Value;
        }

        protected override void OnStop()
        {
            context.agent.isStopped = true;
            if(_lastRequest != null) context.agent.ResetPath();
            _lastRequest = null;
        }
    
        protected override State OnUpdate()
        {
            if(_lastRequest == null || Vector3.Distance(_lastRequest.Value, targetPosition.Value) > 0.1f)
            {
                _lastRequest = targetPosition.Value;
                if (!context.agent.SetDestination(targetPosition.Value)) return State.Failure;
            }
            
            if (!context.agent.pathPending)
            {
                if(context.agent.remainingDistance <= context.agent.stoppingDistance)
                {
                    return State.Success;
                }
            }
            return State.Running;
        }

        protected override void OnReset() { }
    }
}
