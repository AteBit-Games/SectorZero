using UnityEngine;

namespace Runtime.BehaviourTree 
{
    [System.Serializable]
    public class NodeProperty 
    {
        [SerializeReference] public BlackboardKey reference; 
    }

    [System.Serializable]
    public class NodeProperty<T> : NodeProperty 
    {
        public T defaultValue;
        private BlackboardKey<T> _typedKey;

        private BlackboardKey<T> typedKey 
        {
            get 
            {
                if (_typedKey == null && reference != null) _typedKey = reference as BlackboardKey<T>;
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