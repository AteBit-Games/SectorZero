using System;

namespace Runtime.BehaviourTree.Actions.Navigation 
{
    [Serializable]
    [Name("Stop Moving")]
    [Category("Navigation")]
    [Description("Stops the agent from moving")]
    public class StopMoving : ActionNode
    {
        protected override void OnStart() { }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            context.agent.isStopped = true;
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
