using System;

namespace Runtime.BehaviourTree.Conditions.Navigation
{
    [Serializable]
    [Name("Has Path")]
    [Category("Navigation")]
    [Description("Check if a path exists to a point.")]
    public class HasPath : ConditionNode
    {
        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            return context.agent.hasPath ?  State.Success : State.Failure;
        }

        protected override void OnReset() { }
    }
}