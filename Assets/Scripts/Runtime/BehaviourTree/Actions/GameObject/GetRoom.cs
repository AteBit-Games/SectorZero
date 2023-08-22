/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.GameObject 
{
    [Serializable]
    [Name("Get Room From Point")]
    [Category("Navigation")]
    [Description("Selects a random room from a list of rooms")]
    public class GetRoom : ActionNode
    {
        public NodeProperty<Vector2> point;
        public NodeProperty<Collider2D> outRoom;
        
        private bool _roomFound;
        
        protected override void OnStart()
        {
            var hit = Physics2D.OverlapPoint(point.Value, 1 << LayerMask.NameToLayer("RoomBounds"));
            if (hit != null)
            {
                outRoom.Value = hit;
                _roomFound = true;
            }
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            return _roomFound ? State.Success : State.Failure;
        }
        
        protected override void OnReset() { }
    }
}
