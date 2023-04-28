using System;

namespace Runtime.BehaviourTree.Actions.Animation 
{
    [Serializable]
    [Name("Set Float Parameter")]
    [Category("Animation")]
    [Description("Sets a float parameter on the animator")]
    public class SetFloatParameter : ActionNode
    {
        public string parameterName;
        public float setTo;
        
        private float _currentValue;
        
        protected override void OnStart() { }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            if (string.IsNullOrEmpty(parameterName) || context.animator == null)
            {
                return State.Failure;
            }
            
            _currentValue = context.animator.GetFloat(parameterName);
            return Math.Abs(_currentValue - setTo) < 0.01f ? State.Failure : State.Success;
        }
        
        protected override void OnReset() { }
    }
}
