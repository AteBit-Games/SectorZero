using System;
using Runtime.Utils;

namespace Runtime.BehaviourTree.Conditions.Animation 
{
    [Serializable]
    [Name("Check Int")]
    [Category("Animation")]
    [Description("Checks if a int parameter on an AnimatorController is equal to a value")]
    public class CheckIntParameter : ConditionNode
    {
        public string parameter;
        public CompareMethod comparison = CompareMethod.EqualTo;
        public int value;

        protected override void OnStart() { }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            return OperationUtils.Compare(context.animator.GetInteger(parameter), value, comparison) ? State.Success : State.Failure;
        }
        
        protected override void OnReset() { }
    }
}
