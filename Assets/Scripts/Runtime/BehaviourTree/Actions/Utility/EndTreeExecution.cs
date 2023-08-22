/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;

namespace Runtime.BehaviourTree.Actions.Utility 
{
    [Serializable]
    [Name("End Tree Execution")]
    [Category("Utility")]
    [Description("Ends the execution of the tree")]
    public class EndTreeExecution : ActionNode
    {
        protected override void OnStart()
        {
            context.owner.behaviourTree = null;
        }
        
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
