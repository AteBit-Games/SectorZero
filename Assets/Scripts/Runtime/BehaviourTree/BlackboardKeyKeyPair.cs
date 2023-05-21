using UnityEngine;

namespace Runtime.BehaviourTree {
    
    [System.Serializable]
    public class BlackboardKeyPair 
    {
        [SerializeReference] public BlackboardKey source;
        [SerializeReference] public BlackboardKey target;

        public void WriteValue() 
        {
            if (source != null && target != null) target.CopyFrom(source);
        }
    }
}
