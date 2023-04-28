using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.BehaviourTree.Composites 
{
    [Serializable]
    [Name("Parallel")]
    [Category("Composites")]
    [Description("Executes its children in parallel and returns Success if all children return Success or Failure depending on the selected policy.")]
    public class Parallel : CompositeNode 
    {
        private List<State> childrenLeftToExecute = new();

        protected override void OnStart()
        {
            childrenLeftToExecute.Clear();
            children.ForEach(a => {
                childrenLeftToExecute.Add(State.Running);
            });
        }

        protected override void OnStop() { }

        protected override State OnUpdate() 
        {
            bool stillRunning = false;
            for (int i = 0; i < childrenLeftToExecute.Count(); ++i) 
            {
                if (childrenLeftToExecute[i] == State.Running) 
                {
                    var status = children[i].Update();
                    switch (status)
                    {
                        case State.Failure:
                            AbortRunningChildren();
                            return State.Failure;
                        case State.Running:
                            stillRunning = true;
                            break;
                        case State.Inactive: case State.Success:
                            break;  
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    childrenLeftToExecute[i] = status;
                }
            }

            return stillRunning ? State.Running : State.Success;
        }

        protected override void OnReset()
        {
            childrenLeftToExecute.Clear();
            children.ForEach(a => {
                childrenLeftToExecute.Add(State.Running);
            });
        }

        public void AbortRunningChildren() 
        {
            for (int i = 0; i < childrenLeftToExecute.Count(); ++i) 
            {
                if (childrenLeftToExecute[i] == State.Running)  children[i].Abort();
            }
        }
    }
}