/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.Navigation 
{
    [Serializable]
    [Name("Select Room")]
    [Category("Navigation")]
    [Description("Selects a random room from a list of rooms")]
    public class SelectRoom : ActionNode
    {
        public NodeProperty<List<Collider2D>> rooms;
        public NodeProperty<Collider2D> prevRoom;
        public NodeProperty<Collider2D> outRoom;

        protected override void OnStart()
        {
            if (rooms.Value == null)
            {
                UnityEngine.Debug.LogError("No Rooms Specified");
                return;
            }
            
            //select a random room from the list that is not the previous room
            var index = UnityEngine.Random.Range(0, rooms.Value.Count);
            while (rooms.Value[index] == prevRoom.Value)
            {
                index = UnityEngine.Random.Range(0, rooms.Value.Count);
            }
            outRoom.Value = rooms.Value[index];
            prevRoom.Value = outRoom.Value;
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
