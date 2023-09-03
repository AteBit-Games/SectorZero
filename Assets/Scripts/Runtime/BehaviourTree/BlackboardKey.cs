using System;
using UnityEngine;

namespace Runtime.BehaviourTree
{
    [Serializable]
    public abstract class BlackboardKey : ISerializationCallbackReceiver
    {
        public string name;
        public Type underlyingType;
        public string typeName;

        protected BlackboardKey(Type underlyingType)
        {
            this.underlyingType = underlyingType;
            typeName = this.underlyingType.FullName;
        }
        
        public void OnBeforeSerialize() 
        {
            typeName = underlyingType.AssemblyQualifiedName;
            
        }

        public void OnAfterDeserialize() 
        {
            underlyingType = Type.GetType(typeName);
        }

        public abstract void CopyFrom(BlackboardKey key);
        public abstract bool Equals(BlackboardKey key);

        public static BlackboardKey CreateKey(Type type) 
        {
            return Activator.CreateInstance(type) as BlackboardKey;
        }

    }

    [Serializable]
    public abstract class BlackboardKey<T> : BlackboardKey
    {

        public T value;
        protected BlackboardKey() : base(typeof(T)) { }

        public override string ToString()
        {
            return $"{name} : {value}";
        }

        public override void CopyFrom(BlackboardKey key)
        {
            if (key.underlyingType == underlyingType)
            {
                if (key is BlackboardKey<T> other) value = other.value;
            }
        }

        public override bool Equals(BlackboardKey key) 
        {
            if (key.underlyingType == underlyingType) 
            {
                var other = key as BlackboardKey<T>;
                return other != null && value.Equals(other.value);
            }
            return false;
        }

    }
}