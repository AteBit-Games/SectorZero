/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using UnityEngine;

namespace Runtime.BehaviourTree.Monsters 
{
    [DefaultExecutionOrder(10)]
    public class Vincent : BehaviourTreeOwner
    {
        private BlackboardKey<bool> _finalStage;
        
        protected new void Awake() 
        {
            base.Awake();
            
            _finalStage = FindBlackboardKey<bool>("FinalStage");
        }

        public void TriggerFinalStage()
        {
            _finalStage.value = true;   
        }
        
        public void SetNewTree(BehaviourTree tree)
        {
            behaviourTree = tree;
            Awake();
            Start();
        }
    }
}