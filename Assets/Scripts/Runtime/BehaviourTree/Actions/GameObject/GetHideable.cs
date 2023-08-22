/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using System.Collections.Generic;
using Runtime.InteractionSystem.Interfaces;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.GameObject 
{
    [Serializable]
    [Name("Get Hidables")]
    [Category("GameObject")]
    [Description("Get all object the player can hide in within the search radius")]
    public class GetHideable : ActionNode
    {
        public NodeProperty<Collider2D> searchRadius;
        public NodeProperty<List<Collider2D>> outHideables;
        
        protected override void OnStart()
        {
            var overlapResults = new List<Collider2D>(10);
            Physics2D.OverlapCollider(searchRadius.Value, overlapResults);
            
            var hideables = new List<Collider2D>();
            foreach (var collider in overlapResults)
            {
                var hideable = collider.GetComponent<IHideable>();
                if (hideable != null)
                {
                    hideables.Add(collider);
                }
            }
            
            outHideables.Value = hideables;
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
