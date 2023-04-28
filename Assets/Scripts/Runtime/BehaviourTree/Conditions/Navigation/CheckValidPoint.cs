using System;
using Runtime.Navigation;
using UnityEngine;

namespace Runtime.BehaviourTree.Conditions.Navigation
{
    [Serializable]
    [Name("Check Valid Point")]
    [Category("Navigation")]
    [Description("Check if the agent has a valid path")]
    public class CheckValidPoint : ConditionNode
    {
        public new NodeProperty<Vector2> position;

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if(!context.agent.map) return State.Failure;
            return context.agent.map.PointIsValid(position.Value) ? State.Success : State.Failure;
        }

        protected override void OnReset() { }
    }
}