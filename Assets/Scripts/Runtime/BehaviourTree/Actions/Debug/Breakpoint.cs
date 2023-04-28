using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.Debug 
{
    [Serializable]
    [Name("Breakpoint")]
    [Category("Debug")]
    [Description("Trigger a breakpoint when this node is executed")]
    public class Breakpoint : ActionNode
    {
        protected override void OnStart() {
            UnityEngine.Debug.Log("Triggering Breakpoint");
            UnityEngine.Debug.Break();
        }

        protected override void OnStop() { }

        protected override State OnUpdate() 
        {
            return State.Success;
        }

        protected override void OnReset() { }
    }
}
