using System;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.BehaviourTree.Conditions.Navigation
{
    [Serializable]
    [Name("Check Valid Point")]
    [Category("Navigation")]
    [Description("Check if the agent has a valid path")]
    public class CheckValidPoint : ConditionNode
    {
        public NodeProperty<Vector2> positionToCheck;

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            return context.agent.CalculatePath(positionToCheck.Value, new NavMeshPath()) ? State.Success : State.Failure;
        }

        protected override void OnReset() { }
    }
}