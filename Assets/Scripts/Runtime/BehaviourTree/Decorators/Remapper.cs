using System;

namespace Runtime.BehaviourTree.Decorators 
{
    [System.Serializable]
    [Name("Remap")]
    [Category("Decorators")]
    [Description("Remaps the child status to another status. Used to either invert the child's return status or to always return a specific status.")]
    public class Remapper : DecoratorNode 
    {
        public enum RemapStatus
        {
            Failure = 0,
            Success = 1,
        }
        
        public RemapStatus successRemap = RemapStatus.Success;
        public RemapStatus failureRemap = RemapStatus.Failure;


        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate() 
        {
            if (child == null) return State.Failure;
            
            State childState = child.Update();
            switch (childState)
            {
                case State.Success:
                    return (State)successRemap;
                case State.Failure:
                    return (State)failureRemap;
                case State.Inactive:
                    break;
                case State.Running:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return childState;
        }

        protected override void OnReset() { }
    }
}