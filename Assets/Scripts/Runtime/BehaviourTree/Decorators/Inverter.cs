using System;

namespace Runtime.BehaviourTree.Decorators 
{
    [Serializable]
    [Name("Inverter")]
    [Category("Decorators")]
    [Description("Inverts Success to Failure and Failure to Success.")]
    public class Inverter : DecoratorNode 
    {
        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (child == null) return State.Failure;

            return child.Update() switch
            {
                State.Running => State.Running,
                State.Failure => State.Success,
                State.Success => State.Failure,
                State.Inactive => State.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected override void OnReset() { }
    }
}