using System;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.BehaviourTree.Conditions.Navigation
{
    [Serializable]
    [Name("Is Point Valid")]
    [Category("Navigation")]
    [Description("Check if the agent has a valid path")]
    public class ValidPoint : ConditionNode
    {
        public NodeProperty<Vector2> positionToCheck;

        protected override void OnStart()
        {
            context.agent.destination = positionToCheck.Value;
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (context.agent.pathPending) return State.Running;
            return (positionToCheck.Value - (Vector2)context.agent.destination).magnitude > 3f ? State.Failure : State.Success;
        }

        protected override void OnReset() { }
    }
}