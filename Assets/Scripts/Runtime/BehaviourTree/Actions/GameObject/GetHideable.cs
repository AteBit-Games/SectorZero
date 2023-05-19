using System;
using System.Collections.Generic;
using Runtime.InteractionSystem;
using Runtime.InteractionSystem.Interfaces;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions 
{
    [Serializable]
    [Name("Get Hidables")]
    [Category("GameObject")]
    [Description("Get all object the player can hide in within the search radius")]
    public class GetHideable : ActionNode
    {
        public NodeProperty<Collider2D> searchRadius;
        public NodeProperty<LayerMask> objectsLayer;
        public NodeProperty<List<Collider2D>> outHideables;
        
        protected override void OnStart()
        {
            var hideables = new List<Collider2D>();
            if (searchRadius.Value is CircleCollider2D searchCollider)
            {
                var colliders = Physics2D.OverlapCircleAll(searchRadius.Value.transform.position, searchCollider.radius, objectsLayer.Value);
                foreach (var c in colliders)
                {
                    var hideable = c.GetComponent<IHideable>();
                    if (hideable != null)
                    {
                        hideables.Add(c);
                    }
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
