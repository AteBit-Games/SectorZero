using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.Navigation 
{
    [Serializable]
    [Name("Seek (GameObject)")]
    [Category("Navigation")]
    [Description("Seek a GameObject and move towards its position")]
    public class MoveToGameObject : ActionNode
    {
        public NodeProperty<UnityEngine.GameObject> target;
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
            
            return !context.agent.SetDestination(target.Value.transform.position) ? State.Failure : State.Running;
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
