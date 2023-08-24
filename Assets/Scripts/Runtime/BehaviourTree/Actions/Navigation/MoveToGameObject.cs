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
        
        private Vector3? _lastRequest;

        protected override void OnStart()
        {
            context.agent.speed = speed.Value;
            context.agent.isStopped = false;
        }

        protected override void OnStop()
        {
            context.agent.isStopped = true;
            if(_lastRequest != null) context.agent.ResetPath();
            _lastRequest = null;
        }
    
        protected override State OnUpdate()
        {
            if(_lastRequest == null || Vector3.Distance(_lastRequest.Value, target.Value.transform.position) > 0.1f)
            {
                _lastRequest = target.Value.transform.position;
                
                var directionNormalized = target.Value.transform.position - context.agent.transform.position;
                context.owner.SetLookDirection(directionNormalized);
                
                if (!context.agent.SetDestination(target.Value.transform.position)) return State.Failure;
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
