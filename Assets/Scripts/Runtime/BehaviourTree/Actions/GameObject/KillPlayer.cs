using System;
using Runtime.Player;

namespace Runtime.BehaviourTree.Actions.GameObject 
{
    [Serializable]
    [Name("Kill Player")]
    [Category("GameObject")]
    [Description("Kill the player")]
    public class KillPlayer : ActionNode
    {
        public NodeProperty<UnityEngine.GameObject> player;

        protected override void OnStart()
        {
            if(player.Value.TryGetComponent(out PlayerController playerController))
            {
                playerController.Die();
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
