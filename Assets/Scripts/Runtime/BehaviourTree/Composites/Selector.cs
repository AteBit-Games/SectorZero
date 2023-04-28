using System;

namespace Runtime.BehaviourTree.Composites 
{
    [Serializable]
    [Name("Selector")]
    [Category("Composites")]
    [Description("Executes its children in order, returns Failure if all children return Failure. As soon as a child returns Success, the Selector will stop and return Success as well.")]
    public class Selector : CompositeNode 
    {
        protected int currentChildIndex;

        protected override void OnStart() 
        {
            currentChildIndex = 0;
        }

        protected override void OnStop() { }

        protected override State OnUpdate() 
        {
            for (int i = currentChildIndex; i < children.Count; ++i) 
            {
                currentChildIndex = i;
                var child = children[currentChildIndex];

                switch (child.Update()) {
                    case State.Running:
                        return State.Running;
                    case State.Success:
                        return State.Success;
                    case State.Failure:
                        continue;
                    case State.Inactive: default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return State.Failure;
        }

        protected override void OnReset()
        {
            currentChildIndex = 0;
        }
    }
}