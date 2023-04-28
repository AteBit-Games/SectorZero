namespace Runtime.BehaviourTree.Actions.Blackboard 
{
    [System.Serializable]
    [Name("Compare Property")]
    [Category("Blackboard")]
    [Description("Compare a blackboard property with the value specified in the node")]
    public class CompareProperty : ActionNode
    {
        public BlackboardKeyValuePair pair;

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate() 
        {
            BlackboardKey source = pair.value;
            BlackboardKey destination = pair.key;

            if (source != null && destination != null) 
            {
                if (destination.Equals(source)) 
                {
                    return State.Success;
                }
            }

            return State.Failure;
        }

        protected override void OnReset()
        {
            throw new System.NotImplementedException();
        }
    }
}
