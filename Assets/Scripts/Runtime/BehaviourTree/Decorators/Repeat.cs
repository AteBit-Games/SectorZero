using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Decorators
{
    [Serializable]
    [Name("Repeat")]
    [Category("Decorators")]
    [Description("Repeats the child either x times or until it returns the specified status, or forever.")]
    public class Repeat : DecoratorNode
    {
        [Tooltip("Restarts the subtree on success")] public bool restartOnSuccess = true;
        [Tooltip("Restarts the subtree on failure")] public bool restartOnFailure;
        [Tooltip("Maximum number of times the subtree will be repeated. Set to 0 to loop forever")] public int maxRepeats;

        private int iterationCount;

        protected override void OnStart()
        {
            iterationCount = 0;
        }

        protected override void OnStop() { }

        protected override State OnUpdate() 
        {
            if (child == null) {
                return State.Failure;
            }

            switch (child.Update()) 
            {
                case State.Running:
                    break;
                
                case State.Failure:
                    if (restartOnFailure) 
                    {
                        iterationCount++;
                        if (iterationCount == maxRepeats && maxRepeats > 0) return State.Failure;
                        return State.Running;
                    } 
                    return State.Failure;
                
                case State.Success:
                    if (restartOnSuccess)
                    {
                        iterationCount++;
                        if (iterationCount == maxRepeats && maxRepeats > 0) return State.Success;
                        return State.Running;
                    } 
                    return State.Success;

                case State.Inactive: default:
                    throw new ArgumentOutOfRangeException();
            }
            return State.Running;
        }

        protected override void OnReset()
        {
            iterationCount = 0;
        }
    }

    
}
