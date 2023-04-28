using UnityEngine;

namespace Runtime.BehaviourTree 
{
    [System.Serializable]
    public abstract class DecoratorNode : Node 
    {
        [SerializeReference] [HideInInspector] public Node child;
    }
}
