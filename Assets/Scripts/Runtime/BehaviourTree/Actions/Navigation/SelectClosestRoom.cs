using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.Navigation 
{
    [Serializable]
    [Name("Select Closest Room")]
    [Category("Navigation")]
    [Description("Selects the closest room from a list of rooms")]
    public class SelectClosestRoom : ActionNode
    {
        public NodeProperty<Vector2> point;
        public NodeProperty<List<Collider2D>> rooms;
        public NodeProperty<Collider2D> outTarget;

        protected override void OnStart()
        {
            var closestRoom = rooms.Value[0];
            var closestDistance = Vector2.Distance(point.Value, closestRoom.transform.position);
            foreach (var room in rooms.Value)
            {
                var distance = Vector2.Distance(point.Value, room.transform.position);
                if (distance < closestDistance)
                {
                    closestRoom = room;
                    closestDistance = distance;
                }
            }
            outTarget.Value = closestRoom;
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
