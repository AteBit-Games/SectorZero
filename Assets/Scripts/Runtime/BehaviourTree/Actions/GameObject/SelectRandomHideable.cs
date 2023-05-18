using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions 
{
    [Serializable]
    [Name("Select Random Hideable")]
    [Category("GameObject")]
    [Description("Select a random object the player can hide in within the search radius")]
    public class SelectRandomHideable : ActionNode
    {
        public NodeProperty<List<Collider2D>> hideables;
        public NodeProperty<Collider2D> outHidable;
        public NodeProperty<Vector2> hidablePosition;

        protected override void OnStart()
        {
            if (hideables.Value.Count > 0)
            {
                var randomIndex = UnityEngine.Random.Range(0, hideables.Value.Count);
                outHidable.Value = hideables.Value[randomIndex];
                hidablePosition.Value = hideables.Value[randomIndex].transform.position;
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
