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
    
        private bool _destinationReached;

        protected override void OnStart()
        {
            context.agent.maxSpeed = speed.Value;
            context.agent.OnDestinationReached += OnDestinationReached;
        }

        protected override void OnStop()
        {
            context.agent.Stop();
            context.agent.OnDestinationReached -= OnDestinationReached;
        }
    
        protected override State OnUpdate()
        {
            if(_destinationReached)
            {
                _destinationReached = false;
                return State.Success;
            }
            else return !context.agent.SetDestination(targetPosition.Value) ? State.Failure : State.Running;
        }
        
        private void OnDestinationReached()
        {
            _destinationReached = true;
        }

        protected override void OnReset()
        {
            _destinationReached = false;
        }
    }
}
