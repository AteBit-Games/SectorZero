using UnityEngine;

namespace Runtime.BehaviourTree {
    
    [System.Serializable]
    public class BlackboardSubKey
    {
        [SerializeReference] public BlackboardKey source;
        [SerializeReference] public BlackboardKey target;
    }
}
