using System;
using System.Collections;
using Runtime.Utils;
using UnityEngine;

namespace Runtime.BehaviourTree.Conditions.Blackboard
{
    [Serializable]
    [Name("Check Collider")]
    [Category("Blackboard")]
    [Description("Compares two strings")]
    public class CheckCollider : ConditionNode
    {
        public NodeProperty<Collider2D> value;

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            return value.Value == null ? State.Success : State.Failure;
        }

        protected override void OnReset() { }
    }
}