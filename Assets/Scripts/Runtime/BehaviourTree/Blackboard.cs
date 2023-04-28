using System.Collections.Generic;
using UnityEngine;

namespace Runtime.BehaviourTree
{
    [System.Serializable]
    public class Blackboard
    {
        [SerializeReference]
        public List<BlackboardKey> keys = new();
        
        public BlackboardKey Find(string keyName) 
        {
            return keys.Find((key) => key.name == keyName);
        }
        
        public BlackboardKey<T> Find<T>(string keyName) 
        {
            var foundKey = Find(keyName);

            if (foundKey == null)
            {
                Debug.LogWarning($"Failed to find blackboard key, invalid key name:{keyName}");
                return null;
            }

            if (foundKey.underlyingType != typeof(T)) 
            {
                Debug.LogWarning($"Failed to find blackboard key, invalid key type:{typeof(T)}, Expected:{foundKey.underlyingType}");
                return null;
            }

            if (foundKey is not BlackboardKey<T> foundKeyTyped) 
            {
                Debug.LogWarning($"Failed to find blackboard key, casting failed:{typeof(T)}, Expected:{foundKey.underlyingType}");
                return null;
            }
            
            return foundKeyTyped;
        }


        public void SetValue<T>(string keyName, T value) 
        {
            var key = Find<T>(keyName);
            if (key != null) key.value = value;
        }
        
        public T GetValue<T>(string keyName)
        {
            var key = Find<T>(keyName);
            return key != null ? key.value : default;
        }
    }
}