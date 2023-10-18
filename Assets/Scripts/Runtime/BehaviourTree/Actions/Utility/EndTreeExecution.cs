/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using Runtime.BehaviourTree.Monsters;

namespace Runtime.BehaviourTree.Actions.Utility 
{
    [Serializable]
    [Name("End Tree Execution")]
    [Category("Utility")]
    [Description("Ends the execution of the tree")]
    public class EndTreeExecution : ActionNode
    {
        public BehaviourTree tree;
        
        protected override void OnStart()
        {
            if (context.owner is VoidMask voidMask)
            {
                voidMask.SetNewTree(tree);
            }
            else
            {
                UnityEngine.Debug.LogError("EndTreeExecution node can only be used with VoidMask");
            }
        }
        
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
