using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.Navigation 
{
    [Serializable]
    [Name("Select Point")]
    [Category("Navigation")]
    [Description("Selects a random point within the bounds of a room")]
    public class SelectPoint : ActionNode
    {
        public NodeProperty<Collider2D> room;
        public NodeProperty<Vector2> outTarget;
        public float originChance = 0.2f;

        protected override void OnStart()
        {
            if (room.Value == null)
            {
                UnityEngine.Debug.LogError("No Room Specified");
                return;
            }
            
            if(UnityEngine.Random.Range(0f, 1f) < originChance)
            {
                outTarget.Value = room.Value.transform.position;
            }
            else
            {
                //select a random point within the bounds of the room
                var bounds = room.Value.bounds;
                var x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
                var y = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
                outTarget.Value = new Vector2(x, y);
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
