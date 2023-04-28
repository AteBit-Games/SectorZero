using System;
using Runtime.Utils;
using UnityEngine;

namespace Runtime.BehaviourTree.Composites 
{
    [Serializable]
    [Name("Sequencer")]
    [Category("Composites")]
    [Description("Executes its children in order, returns Success if all children return Success. As soon as a child returns Failure, the Sequencer will stop and return Failure as well.")]
    public class Sequencer : CompositeNode 
    {
        protected int currentChildIndex;

        protected override void OnStart() 
        {
            currentChildIndex = 0;
        }

        protected override void OnStop() { }

        protected override State OnUpdate() {
            for (int i = currentChildIndex; i < children.Count; ++i) {
                currentChildIndex = i;
                var child = children[currentChildIndex];

                switch (child.Update()) {
                    case State.Running:
                        return State.Running;
                    case State.Failure:
                        return State.Failure;
                    case State.Success:
                        continue;
                    case State.Inactive:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return State.Success;
        }
        
        protected override void OnReset() 
        {
            currentChildIndex = 0;
        }
        
    }
}