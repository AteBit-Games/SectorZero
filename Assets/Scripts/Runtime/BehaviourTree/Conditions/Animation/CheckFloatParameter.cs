using System;
using Runtime.Utils;

namespace Runtime.BehaviourTree.Conditions.Animation 
{
    [Serializable]
    [Name("Check Float")]
    [Category("Animation")]
    [Description("Checks if a float parameter on an AnimatorController is equal to a value")]
    public class CheckFloatParameter : ConditionNode
    {
        public string parameter;
        public CompareMethod comparison = CompareMethod.EqualTo;
        public float value;

        protected override void OnStart() { }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            return OperationUtils.Compare(context.animator.GetFloat(parameter), value, comparison, 0.1f) ? State.Success : State.Failure;
        }
        
        protected override void OnReset() { }
    }
}
