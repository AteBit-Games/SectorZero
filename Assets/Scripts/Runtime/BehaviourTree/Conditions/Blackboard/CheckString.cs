using System;
using System.Collections;
using Runtime.Utils;
using UnityEngine;

namespace Runtime.BehaviourTree.Conditions.Blackboard
{
    [Serializable]
    [Name("Check String")]
    [Category("Blackboard")]
    [Description("Compares two strings")]
    public class CheckString : ConditionNode
    {
        public NodeProperty<string> valueA;
        public NodeProperty<string> valueB;

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            return valueA.Value == valueB.Value ? State.Success : State.Failure;
        }

        protected override void OnReset() { }
    }
}