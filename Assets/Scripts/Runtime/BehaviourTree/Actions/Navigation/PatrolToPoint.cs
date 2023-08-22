/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.Navigation 
{
    [Serializable]
    [Name("Patrol To Point")]
    [Category("Navigation")]
    [Description("Seek a target position")]
    public class PatrolToPoint : ActionNode
    {
        public NodeProperty<Vector2> targetPoint;
        public NodeProperty<float> speed = new(){Value = 4f};
        public NodeProperty<float> failureChance = new(){Value = 0.1f};
        public NodeProperty<float> failureDistanceRemaining = new(){Value = 0.25f};
        
        private bool _willFail;
        private float _totalDistance;

        protected override void OnStart()
        {
            context.agent.speed = speed.Value;
            context.agent.destination = targetPoint.Value;
            context.agent.isStopped = false;
            
            _totalDistance = Vector3.Distance(context.agent.transform.position, context.agent.destination);
            _willFail = UnityEngine.Random.Range(0f, 1f) < failureChance.Value;
        }

        protected override void OnStop(){ }
    
        protected override State OnUpdate()
        {
            var direction = context.agent.steeringTarget - context.agent.transform.position;
            context.owner.SetLookDirection(direction.normalized);
            
            if (context.agent.pathPending) return State.Running;
            if (_willFail && Vector3.Distance(context.agent.transform.position, context.agent.destination)/_totalDistance < failureDistanceRemaining.Value)
            {
                context.agent.isStopped = true;
                return State.Failure;
            }
            
            if (context.agent.remainingDistance < context.agent.stoppingDistance) return State.Success;
            
            return context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid ? State.Failure : State.Running;
        }

        protected override void OnReset() { }
    }
}
