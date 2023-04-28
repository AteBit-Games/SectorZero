using UnityEngine;

namespace Runtime.BehaviourTree {
    
    [System.Serializable]
    public class BlackboardKeyValuePair 
    {
        [SerializeReference] public BlackboardKey key;
        [SerializeReference] public BlackboardKey value;

        public void WriteValue() 
        {
            if (key != null && value != null) key.CopyFrom(value);
        }
    }
}
