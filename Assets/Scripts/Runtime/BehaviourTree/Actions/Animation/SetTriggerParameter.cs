using System;

namespace Runtime.BehaviourTree.Actions.Animation 
{
    [Serializable]
    [Name("Set Float Parameter")]
    [Category("Animation")]
    [Description("Sets a float parameter on the animator")]
    public class SetTriggerParameter : ActionNode
    {
        public string parameterName;
        
        protected override void OnStart() { }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            if (string.IsNullOrEmpty(parameterName) || context.animator == null)
            {
                return State.Failure;
            }
            
            context.animator.SetTrigger(parameterName);
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
