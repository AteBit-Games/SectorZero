using System;
using System.Collections;

namespace Runtime.BehaviourTree.Conditions.Blackboard
{
    [Serializable]
    [Name("List Is Empty")]
    [Category("Blackboard")]
    [Description("Check if a list is empty")]
    public class ListIsEmpty : ConditionNode
    {
        public NodeProperty<IList> targetList = new();

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            return targetList.Value.Count == 0 ? State.Success : State.Failure;
        }

        protected override void OnReset() { }
    }
}