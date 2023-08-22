using System;
using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.GameObject 
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
            var direction = hideable.Value.transform.position - context.agent.transform.position;
            context.owner.SetLookDirection(direction.normalized);
            
            if(hideable.Value.TryGetComponent(out IHideable hideableComponent) && hideable.Value.TryGetComponent(out IInteractable interactable))
            {
                var source = hideable.Value.GetComponent<AudioSource>();
                GameManager.Instance.SoundSystem.Play(interactable.InteractSound, source);
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
