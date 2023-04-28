using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.Animation 
{
    [Serializable]
    [Name("Set Bool Parameter")]
    [Category("Animation")]
    [Description("Sets a boolean parameter on the animator")]
    public class SetBoolParameter : ActionNode
    {
        public string parameterName;
        public bool setTo;
        
        protected override void OnStart() { }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            if (string.IsNullOrEmpty(parameterName) || context.animator == null)
            {
                return State.Failure;
            }
            
            context.animator.SetBool(parameterName, setTo);
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
