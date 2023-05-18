using System;
using UnityEngine;

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
            if(!context.agent.map) return State.Failure;
            return context.agent.map.PointIsValid(positionToCheck.Value) ? State.Success : State.Failure;
        }

        protected override void OnReset() { }
    }
}