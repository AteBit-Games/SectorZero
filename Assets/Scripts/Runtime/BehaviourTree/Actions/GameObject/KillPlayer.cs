using System;
using Runtime.Player;
using Runtime.Utils;

namespace Runtime.BehaviourTree.Actions.GameObject 
{
    [Serializable]
    [Name("Kill Player")]
    [Category("GameObject")]
    [Description("Kill the player")]
    public class KillPlayer : ActionNode
    {
        public NodeProperty<UnityEngine.GameObject> player;
        public DeathType deathType;

        protected override void OnStart()
        {
            if(player.Value.TryGetComponent(out PlayerController playerController))
            {
                playerController.Die(deathType);
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
