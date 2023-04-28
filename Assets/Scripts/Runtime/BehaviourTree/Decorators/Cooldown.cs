using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Decorators 
{
    [Serializable]
    [Name("Cooldown")]
    [Category("Decorators")]
    [Description("Waits the specified duration after the child has completed before returning the child's status of success or failure.")]
    public class Cooldown : DecoratorNode
    {
        public NodeProperty<float> duration = new(){Value = 2};
        private float cooldownTime = -1;


        protected override void OnStart()
        {
            cooldownTime = -1;
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            State childState = child.Update();
            //Waits the specified duration after the child has completed before returning the child's status of success or failure
            if (Math.Abs(cooldownTime - (-1)) < 0.0001) cooldownTime = Time.time;
            else if (cooldownTime + duration.Value > Time.time) return childState;
                
            return childState;
        }

        protected override void OnReset()
        {
            cooldownTime = -1;
        }
    }
}