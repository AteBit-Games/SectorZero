using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Conditions.Blackboard
{
    [Serializable]
    [Name("Check Null")]
    [Category("Blackboard")]
    [Description("Check whether or not a variable is null")]
    public class CheckNull : ConditionNode
    {
        public NodeProperty variable;

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            Debug.Log(variable.Equals(null));
            return variable.reference == null ? State.Success : State.Failure;
        }

        protected override void OnReset() { }
    }
}