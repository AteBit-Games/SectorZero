using UnityEngine;

namespace Runtime.BehaviourTree.Decorators 
{
    [Name("Timeout")]
    [Category("Decorators")]
    [Description("Interrupts decorated child node and returns Failure if the child node is still Running after the timeout period.")]
    [System.Serializable]
    public class Timeout : DecoratorNode
    {
        [Tooltip("The timeout period in seconds.")]
        public NodeProperty<float> timeout = new(){Value = 1};
        
        private float startTime;

        protected override void OnStart() 
        {
            startTime = Time.time;
        }

        protected override void OnStop() { }

        protected override State OnUpdate() 
        {
            if (child == null) return State.Failure;
            return Time.time - startTime > timeout.Value ? State.Failure : child.Update();
        }

        protected override void OnReset()
        {
            throw new System.NotImplementedException();
        }
    }
}