using System;
using Runtime.InteractionSystem;
using Runtime.InteractionSystem.Interfaces;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions 
{
    [Serializable]
    [Name("Check Hideable")]
    [Category("GameObject")]
    [Description("Check if the player is hiding in a hideable object")]
    public class CheckHideable : ActionNode
    {
        public NodeProperty<Collider2D> hideable;
        private bool _isHiding;
        
        protected override void OnStart()
        {
            if(hideable.Value.TryGetComponent(out IHideable hideableComponent))
            {
                _isHiding = hideableComponent.ContainsPlayer;
            }
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            return _isHiding ? State.Success : State.Failure;
        }
        
        protected override void OnReset() { }
    }
}
