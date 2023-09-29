using System;
using Runtime.BehaviourTree.Monsters;

namespace Runtime.BehaviourTree.Actions.Blackboard
{
    [Serializable]
    [Name("Set State")]
    [Category("Blackboard")]
    [Description("Sets the state of the monster")]
    public class SetState : ActionNode
    {
        public new MonsterState state;

        protected override void OnStart()
        {
            context.owner.SetState(state);
        }

        protected override void OnStop() { }

        protected override State OnUpdate() 
        {
            return State.Success;
        }

        protected override void OnReset() { }
    }
}
