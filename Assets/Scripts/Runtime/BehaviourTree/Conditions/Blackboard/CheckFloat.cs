using System;
using System.Collections;
using Runtime.Utils;
using UnityEngine;

namespace Runtime.BehaviourTree.Conditions.Blackboard
{
    [Serializable]
    [Name("Check Float")]
    [Category("Blackboard")]
    [Description("Compares two float values")]
    public class CheckFloat : ConditionNode
    {
        public NodeProperty<float> valueA;
        public NodeProperty<float> valueB;
        public CompareMethod checkType = CompareMethod.EqualTo;
        
        [Range(0, 0.1f)]
        public float differenceThreshold = 0.05f;

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            return OperationUtils.Compare(valueA.Value, valueB.Value, checkType, differenceThreshold) ? State.Success : State.Failure;
        }

        protected override void OnReset() { }
    }
}