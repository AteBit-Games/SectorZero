/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;


namespace Runtime.BehaviourTree.Actions.Navigation 
{
    [Serializable]
    [Name("Follow Player")]
    [Category("Navigation")]
    [Description("Seek towards the player")]
    public class FollowPlayer : ActionNode
    {
        public NodeProperty<UnityEngine.GameObject> playerRef;
        public NodeProperty<float> speed = new(){Value = 4f};
        public NodeProperty<float> stoppingDistance = new(){Value = 1f};

        protected override void OnStart()
        {
            context.agent.speed = speed.Value;
            context.agent.stoppingDistance = stoppingDistance.Value;
            context.agent.isStopped = false;
        }

        protected override void OnStop(){ }
    
        protected override State OnUpdate()
        {
            context.agent.destination = playerRef.Value.transform.position;
            
            var direction = context.agent.steeringTarget - context.agent.transform.position;
            context.owner.SetLookDirection(direction.normalized);
            
            if (context.agent.pathPending) return State.Running;
            if (context.agent.remainingDistance < context.agent.stoppingDistance) return State.Success;
            return context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid ? State.Failure : State.Running;
        }

        protected override void OnReset() { }
    }
}
