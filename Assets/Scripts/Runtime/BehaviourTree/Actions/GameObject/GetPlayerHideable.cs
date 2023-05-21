using System;
using Runtime.InteractionSystem.Interfaces;
using Runtime.Player;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.GameObject 
{
    [Serializable]
    [Name("Get Player Hideable")]
    [Category("GameObject")]
    [Description("Get the object the player is hiding in")]
    public class GetPlayerHideable : ActionNode
    {
        public NodeProperty<UnityEngine.GameObject> player;
        public NodeProperty<Collider2D> outHideableObject;
        public NodeProperty<Vector2> outHideablePosition;

        protected override void OnStart()
        {
            var playerController = player.Value.GetComponent<PlayerController>();
            if (playerController.isHiding)
            {
                outHideableObject.Value = playerController.hideable;
                outHideablePosition.Value = playerController.hideable.GetComponent<IHideable>().InspectPosition.position;
            }
            else
            {
                outHideableObject.Value = null;
                outHideablePosition.Value = Vector2.zero;
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
