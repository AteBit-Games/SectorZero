using UnityEngine;

namespace Runtime.BehaviourTree 
{
    [System.Serializable]
    [Name("Root")]
    [Description("The root node of a behaviour tree. Links to the starting point of the tree.")]
    public class RootNode : Node 
    {
        [SerializeReference] [HideInInspector] public Node child;

        protected override void OnStart()
        {
            state = State.Running;
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            return child?.Update() ?? State.Failure;
        }

        protected override void OnReset() { child?.Reset(); }
    }
}