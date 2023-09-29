using System;
using Runtime.BehaviourTree.Monsters;

namespace Runtime.BehaviourTree.Actions.Blackboard
{
    [Serializable]
    [Name("Set State")]
    [Category("Blackboard")]
    [Description("Sets the state of the monster")]
    public class SetCurrentState : ActionNode
    {
        public MonsterState newState;

        protected override void OnStart()
        {
            context.owner.SetState(newState);
        }

        protected override void OnStop() { }

        protected override State OnUpdate() 
        {
            return State.Success;
        }

        protected override void OnReset() { }
    }
}
