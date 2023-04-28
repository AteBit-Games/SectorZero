using System;
using System.Collections;

namespace Runtime.BehaviourTree.Conditions.Blackboard
{
    [Serializable]
    [Name("Check Null")]
    [Category("Blackboard")]
    [Description("Check whether or not a variable is null")]
    public class CheckNull : ConditionNode
    {
        public NodeProperty<object> variable;

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            return variable.Value == null ? State.Success : State.Failure;
        }

        protected override void OnReset() { }
    }
}