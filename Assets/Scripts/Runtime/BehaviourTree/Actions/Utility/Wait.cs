using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.Utility 
{
    [Serializable]
    [Name("Wait")]
    [Category("Utility")]
    [Description("Wait for a specified amount of time")]
    public class Wait : ActionNode 
    {
        [Tooltip("Amount of time to wait before returning success")] 
        public NodeProperty<float> duration = new();
        private float _startTime;

        protected override void OnStart() 
        {
            _startTime = Time.time;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            float timeRemaining = Time.time - _startTime;
            return timeRemaining > duration.Value ? State.Success : State.Running;
        }

        protected override void OnReset() { }
    }
}
