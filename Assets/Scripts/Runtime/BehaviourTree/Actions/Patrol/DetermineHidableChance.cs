/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.Patrol 
{
    [Serializable]
    [Name("Determine Hidable Chance")]
    [Category("Patrol")]
    [Description("Determine the chance of searching a hidable object")]
    public class DetermineHidableChance : ActionNode
    {
        public NodeProperty<int> aggroLevel;
        //did see player enter room and lose them - making them search one more extra
        public NodeProperty<bool> lostPlayer;
        public NodeProperty<float> outChance;

        protected override void OnStart()
        {
            var baseChance = 0.1f;
            
            switch (aggroLevel.Value)
            {
                case <= 3:
                    baseChance = 0.1f;
                    break;
                case <= 9:
                    baseChance = 0.12f;
                    break;
                case 10:
                    baseChance = 0.16f;
                    break;
            }
            
            if(lostPlayer.Value) baseChance += 0.05f;
            outChance.Value = 1 - Mathf.Clamp(baseChance, 0.1f, 0.2f);
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
