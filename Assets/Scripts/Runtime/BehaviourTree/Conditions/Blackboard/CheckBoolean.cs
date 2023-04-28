using System;
using System.Collections;

namespace Runtime.BehaviourTree.Conditions.Blackboard
{
    [Serializable]
    [Name("Check Boolean")]
    [Category("Blackboard")]
    [Description("Compare two boolean values")]
    public class CheckBoolean : ConditionNode
    {
        public NodeProperty<bool> valueA = new();
        public NodeProperty<bool> valueB = new(){Value = true};

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            return valueA.Value == valueB.Value ? State.Success : State.Failure;
        }

        protected override void OnReset() { }
    }
}