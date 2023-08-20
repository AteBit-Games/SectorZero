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
    [Name("Select Patrol Rooms")]
    [Category("Navigation")]
    [Description("Select the next rooms to patrol based on the active patrol state")]
    public class SelectPatrolRooms : ActionNode
    {
        public NodeProperty<UnityEngine.GameObject> target;
        public NodeProperty<List<Collider2D>> rooms;
        public NodeProperty<int> numberOfRooms;
        [Tooltip("Patrol state (False | Close State --- True | Far State)")] public NodeProperty<bool> patrolState;
        public NodeProperty<List<Collider2D>> resultingRooms;

        private List<Collider2D> _selectedRooms;

        protected override void OnStart()
        {
            //Select the number of rooms based on the patrol state
            if (patrolState.Value)
            {
                //select the number of rooms closest to the closest room
                _selectedRooms = new List<Collider2D>(rooms.Value);
                _selectedRooms.Sort((a, b) =>
                {
                    if (Vector2.Distance(target.Value.transform.position, a.transform.position) >
                        Vector2.Distance(target.Value.transform.position, b.transform.position))
                    {
                        return 1;
                    }
                    
                    if (Vector2.Distance(target.Value.transform.position, a.transform.position) < Vector2.Distance(target.Value.transform.position, b.transform.position))
                    {
                        return -1;
                    }
                    
                    return 0;
                });
                
                resultingRooms.Value = _selectedRooms.GetRange(0, numberOfRooms.Value);
            }
            else
            {
                //select the furthest room from target
                Collider2D furthestRoom = null;
                foreach (var room in rooms.Value)
                {
                    if (furthestRoom == null)
                    {
                        furthestRoom = room;
                    }
                    else
                    {
                        if (Vector2.Distance(target.Value.transform.position, room.transform.position) >
                            Vector2.Distance(target.Value.transform.position, furthestRoom.transform.position))
                        {
                            furthestRoom = room;
                        }
                    }
                }
                
                //select the number of rooms closest to the furthest room
                _selectedRooms = new List<Collider2D>(rooms.Value);
                _selectedRooms.Sort((a, b) =>
                {
                    if (Vector2.Distance(furthestRoom.transform.position, a.transform.position) >
                        Vector2.Distance(furthestRoom.transform.position, b.transform.position))
                    {
                        return 1;
                    }
                    
                    if (Vector2.Distance(furthestRoom.transform.position, a.transform.position) < Vector2.Distance(furthestRoom.transform.position, b.transform.position))
                    {
                        return -1;
                    }
                    
                    return 0;
                });
                
                resultingRooms.Value = _selectedRooms.GetRange(0, numberOfRooms.Value);
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
