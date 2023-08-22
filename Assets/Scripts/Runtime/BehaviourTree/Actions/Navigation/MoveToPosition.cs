/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

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
            context.agent.destination = targetPosition.Value;
            context.agent.isStopped = false;
        }

        protected override void OnStop(){ }
    
        protected override State OnUpdate()
        {
            var direction = context.agent.steeringTarget - context.agent.transform.position;
            context.owner.SetLookDirection(direction.normalized);
            
            if (context.agent.pathPending) return State.Running;
            if (context.agent.remainingDistance < context.agent.stoppingDistance) return State.Success;
            
            return context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid ? State.Failure : State.Running;
        }

        protected override void OnReset() { }
    }
}
