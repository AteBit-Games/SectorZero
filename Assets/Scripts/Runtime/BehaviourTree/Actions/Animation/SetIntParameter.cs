using System;

namespace Runtime.BehaviourTree.Actions.Animation 
{
    [Serializable]
    [Name("Set Int Parameter")]
    [Category("Animation")]
    [Description("Sets a int parameter on the animator")]
    public class SetIntParameter : ActionNode
    {
        public string parameterName;
        public int setTo;
        
        protected override void OnStart() { }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            if (string.IsNullOrEmpty(parameterName) || context.animator == null)
            {
                return State.Failure;
            }
            
            context.animator.SetInteger(parameterName, setTo);
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
