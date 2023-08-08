using System.Collections.Generic;
using Runtime.BehaviourTree.Actions;
using UnityEngine;

namespace Runtime.BehaviourTree 
{
    [System.Serializable]
    [Name("Sub Tree")]
    [Description("Runs a behaviour tree as a subtree")]
    public class SubTree : Node
    {
        [Tooltip("Behaviour tree asset to run as a subtree")] public BehaviourTree treeAsset;
        [HideInInspector] public BehaviourTree treeInstance;
        public List<BlackboardSubKey> blackboardOverrides = new();
        
        protected override void OnStart()
        {
            treeInstance = treeAsset.Clone();
            treeInstance.Bind(context);
            treeInstance.treeState = State.Running;
            ApplyKeyOverrides();
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            return treeInstance ? treeInstance.Update() : State.Failure;
        }

        protected override void OnReset() { }
        
        private void ApplyKeyOverrides() 
        {
            foreach(var pair in blackboardOverrides)
            {
                var target = treeInstance.blackboard.Find(pair.target.name);
                var source = blackboard.Find(pair.source.name);
                if (target != null && source != null) target.CopyFrom(source);
            }
        }
    }
}