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
    
    public enum OperationMethod
    {
        Set,
        Add,
        Subtract,
        Multiply,
        Divide
    }
    
    public enum CollisionTypes
    {
        CollisionEnter = 0,
        CollisionExit = 1,
        CollisionStay = 2
    }
}