using System;

namespace Runtime.BehaviourTree.Conditions.Animation
{
    [Serializable]
    [Name("Check Bool")]
    [Category("Animation")]
    [Description("Checks if a bool parameter on an AnimatorController is true or false")]
    public class CheckBoolParameter : ConditionNode
    {
        public string parameter;
        public bool value;
        
        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            return context.animator.GetBool(parameter) == value ? State.Success : State.Failure;
        }

        protected override void OnReset() { }
    }
}