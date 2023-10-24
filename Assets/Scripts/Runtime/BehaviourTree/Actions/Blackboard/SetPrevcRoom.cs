using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.Blackboard
{
    [Serializable]
    [Name("Set Previous Room")]
    [Category("Blackboard")]
    [Description("Sets the previous room to the current room")]
    public class SetPrevRoom : ActionNode
    {
        public NodeProperty<Collider2D> prevRoom;
        public NodeProperty<Collider2D> currRoom;

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate() 
        {
            prevRoom.Value = currRoom.Value;
            return State.Success;
        }

        protected override void OnReset() { }
    }
}
