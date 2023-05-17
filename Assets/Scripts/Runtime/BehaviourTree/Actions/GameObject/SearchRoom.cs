using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.GameObject 
{
    [Serializable]
    [Name("Select Point")]
    [Category("GameObject")]
    [Description("Selects a random point within the bounds of the room.")]
    public class SelectPoint : ActionNode
    {
        public NodeProperty<Collider2D> room;
        public NodeProperty<Vector2> outTarget;
        
        protected override void OnStart() { }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            //select a random point within the bounds of the room
            var bounds = room.Value.bounds;
            var x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
            var y = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
            outTarget.Value = new Vector2(x, y);
            
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
