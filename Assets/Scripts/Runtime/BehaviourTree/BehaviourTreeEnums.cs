namespace Runtime.BehaviourTree
{
    public enum CompareMethod
    {
        EqualTo,
        GreaterThan,
        LessThan,
        GreaterOrEqualTo,
        LessOrEqualTo
    }
    
    public enum MonsterStates
    {
        Idle,
        Patrol,
        Attack,
        Flee,
        Suspicious,
        Search,
        Wander,
        Alert,
        InvestigateSound,
        InvestigateSight,
    }
}