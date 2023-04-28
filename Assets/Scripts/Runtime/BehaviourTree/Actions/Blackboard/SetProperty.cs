using System;

namespace Runtime.BehaviourTree.Actions.Blackboard
{
    [Serializable]
    [Name("Set Property")]
    [Category("Blackboard")]
    [Description("Set a property on the blackboard")]
    public class SetProperty : ActionNode
    {
        public BlackboardKeyValuePair pair;

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate() 
        {
            pair.WriteValue();
            return State.Success;
        }

        protected override void OnReset() { }
    }
}
