using UnityEngine;

namespace Runtime.BehaviourTree {
    
    [System.Serializable]
    public class BlackboardSubKey 
    {
        [SerializeReference] public BlackboardKey source;
        [SerializeReference] public BlackboardKey target;

        public void WriteValue() 
        {
            if (source != null && target != null) target.CopyFrom(source);
        }
    }
    
    [System.Serializable]
    public class BlackboardSubKey<T> : BlackboardSubKey 
    {
        public T defaultValue;
        private BlackboardKey<T> _typedKey;

        private BlackboardKey<T> typedKey 
        {
            get 
            {
                if (_typedKey == null && source != null) _typedKey = source as BlackboardKey<T>;
                return _typedKey;
            }
        }

        public T Value 
        {
            set 
            {
                if (typedKey != null) typedKey.value = value;
                else defaultValue = value;
            }
            get => typedKey != null ? typedKey.value : defaultValue;
        }
    }
}
