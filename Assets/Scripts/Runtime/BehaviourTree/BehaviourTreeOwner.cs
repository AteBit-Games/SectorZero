using System.Collections.Generic;
using UnityEngine;

namespace Runtime.BehaviourTree 
{
    [AddComponentMenu("BehaviourTree/BehaviourTreeOwner")]
    public class BehaviourTreeOwner : MonoBehaviour 
    {
        [Tooltip("BehaviourTree asset to instantiate during Awake")] 
        public BehaviourTree behaviourTree;
        
        [Tooltip("Run behaviour tree validation at startup")] 
        public bool validate = true;
        
        [Tooltip("Override blackboard values from the behaviour tree asset")]
        public List<BlackboardKeyValuePair> blackboardOverrides = new();
        private Context context;

        private void Awake() 
        {
            bool isValid = ValidateTree();
            if (isValid) 
            {
                context = CreateBehaviourTreeContext();
                behaviourTree = behaviourTree.Clone();
                behaviourTree.Bind(context);
                ApplyKeyOverrides();
            }
            else
            {
                behaviourTree = null;
            }
        }

        private void ApplyKeyOverrides() 
        {
            foreach(var pair in blackboardOverrides)
            {
                var targetKey = behaviourTree.blackboard.Find(pair.key.name);
                var sourceKey = pair.value;
                if (targetKey != null && sourceKey != null) targetKey.CopyFrom(sourceKey);
            }
        }

        private void Update() 
        {
            if (behaviourTree) behaviourTree.Update();
        }

        private Context CreateBehaviourTreeContext() 
        {
            return Context.CreateFromGameObject(gameObject);
        }

        private bool ValidateTree() 
        {
            if (!behaviourTree) 
            {
                Debug.LogWarning($"No BehaviourTree assigned to {name}, assign a behaviour tree in the inspector");
                return false;
            }

            return true;
        }
        
        private void OnDrawGizmos() 
        {
            if (!behaviourTree || !Application.isPlaying) return;

            BehaviourTree.Traverse(behaviourTree.rootNode, node =>
            {
                if(node.drawGizmos) node.OnDrawGizmos();
            });
        }
        
        public BlackboardKey<T> FindBlackboardKey<T>(string keyName)
        {
            return behaviourTree ? behaviourTree.blackboard.Find<T>(keyName) : null;
        }

        public void SetBlackboardValue<T>(string keyName, T value)
        {
            if (behaviourTree) behaviourTree.blackboard.SetValue(keyName, value);
        }

        public T GetBlackboardValue<T>(string keyName)
        {
            return behaviourTree ? behaviourTree.blackboard.GetValue<T>(keyName) : default;
        }
    }
}