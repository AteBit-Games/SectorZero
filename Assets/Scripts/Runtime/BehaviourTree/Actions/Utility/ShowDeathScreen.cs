using System;
using Runtime.Managers;
using Runtime.Utils;

namespace Runtime.BehaviourTree.Actions.Utility 
{
    [Serializable]
    [Name("Show Death Screen")]
    [Category("Utility")]
    [Description("Show the death screen")]
    public class ShowDeathScreen : ActionNode
    {
        public NodeProperty<DeathType> deathType;
        
        protected override void OnStart()
        {
            GameManager.Instance.GameOver(deathType.Value);
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
