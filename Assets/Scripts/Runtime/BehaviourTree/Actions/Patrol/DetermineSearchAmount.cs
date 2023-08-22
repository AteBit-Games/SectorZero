/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.BehaviourTree.Actions.Patrol 
{
    [Serializable]
    [Name("Determine Search Amount")]
    [Category("Patrol")]
    [Description("Determine the speed of the agent based on the aggro level")]
    public class DetermineSearchAmount : ActionNode
    {
        public NodeProperty<Collider2D> room;
        public NodeProperty<int> aggroLevel;
        public NodeProperty<bool> lostPlayer;
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
                    times = Mathf.RoundToInt(Random.Range(2, 3));
                    if(aggroLevel.Value > 6 || lostPlayer.Value) Math.Clamp(times = times + Random.value < 0.5f ? 1 : 0, 2, 3);
                    break;
                case > 225:
                    times = Mathf.RoundToInt(Random.Range(2, 4));
                    if(aggroLevel.Value > 6 || lostPlayer.Value) Math.Clamp(times = times + Random.value < 0.5f ? 1 : 0, 2, 4);
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
