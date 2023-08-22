/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.Patrol 
{
    [Serializable]
    [Name("Determine Look Amount")]
    [Category("Patrol")]
    [Description("Determine the speed of the agent based on the aggro level")]
    public class DetermineLookAmount : ActionNode
    {
        public NodeProperty<int> aggroLevel;
        public NodeProperty<Collider2D> room;
        public NodeProperty<int> outTimes;
        

        protected override void OnStart()
        {
            var roomSize = room.Value.bounds.size;
            var roomArea = roomSize.x * roomSize.y;

            var times = 0;
            switch (roomArea)
            {
                case < 100:
                    times = 1;
                    break;
                case > 100 and < 225:
                    times = 2;
                    break;
                case > 225:
                    times = 3;
                    break;
            }
            
            outTimes.Value = times;
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate()
        {
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
