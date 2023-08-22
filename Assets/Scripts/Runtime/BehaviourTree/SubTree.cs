using System.Collections.Generic;
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
        public bool cloneInstance = true;
        public List<BlackboardSubKey> blackboardOverrides = new();
        
        private bool _instanced;
        
        protected override void OnStart()
        {
            //if clone instance is true, create a new instance of the tree asset every time this node is started else use the same instance use instanced
            if (cloneInstance)
            {
                treeInstance = treeAsset.Clone();
            }
            else
            {
                if (!_instanced)
                {
                    treeInstance = treeAsset.Clone();
                    _instanced = true;
                }
            }
            
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

                if (pair.source == null)
                {
                    target.CopyFrom(pair.target);
                }
                else
                {
                    var source = blackboard.Find(pair.source.name);
                    if (target != null && source != null) target.CopyFrom(source);
                }
            }
        }
    }
}