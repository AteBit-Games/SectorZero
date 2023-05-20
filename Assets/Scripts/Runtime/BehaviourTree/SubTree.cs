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

        public SubTree(BehaviourTree treeAsset)
        {
            this.treeAsset = treeAsset;
        }
        
        protected override void OnStart()
        {
            treeInstance = treeAsset.Clone();
            treeInstance.Bind(context);
            treeInstance.treeState = State.Running;
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            return treeInstance ? treeInstance.Update() : State.Failure;
        }

        protected override void OnReset() { }
    }

}