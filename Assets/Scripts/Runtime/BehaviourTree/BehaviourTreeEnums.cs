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
        Idle = 0,
        Alert = 1,
        Aggro = 2,
        // Patrol,
        // Attack,
        // Flee,
        // Suspicious,
        // Search,
        // Wander,
        // InvestigateSound,
        // InvestigateSight,
    }
}