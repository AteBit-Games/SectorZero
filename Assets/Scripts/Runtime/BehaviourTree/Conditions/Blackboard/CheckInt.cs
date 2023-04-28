using System;
using System.Collections;
using Runtime.Utils;
using UnityEngine;

namespace Runtime.BehaviourTree.Conditions.Blackboard
{
    [Serializable]
    [Name("Check Int")]
    [Category("Blackboard")]
    [Description("Compares two int values")]
    public class CheckInt : ConditionNode
    {
        public NodeProperty<int> valueA;
        public NodeProperty<int> valueB;
        public CompareMethod checkType = CompareMethod.EqualTo;
        
        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            return OperationUtils.Compare(valueA.Value, valueB.Value, checkType) ? State.Success : State.Failure;
        }

        protected override void OnReset() { }
    }
}