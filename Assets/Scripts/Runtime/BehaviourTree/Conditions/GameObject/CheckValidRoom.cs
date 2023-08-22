using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Conditions.GameObject 
{
    [Serializable]
    [Name("Check Valid Room")]
    [Category("GameObject")]
    [Description("Check whether or not a room is valid")]
    public class CheckValidRoom : ConditionNode
    {
        public NodeProperty<Collider2D> room;

        protected override void OnStart() { }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate()
        {
            return room.Value == null ? State.Success : State.Failure;
        }
        
        protected override void OnReset() { }
    }
}
