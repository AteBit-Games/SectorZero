using System.Collections.Generic;
using System.Linq;
using Runtime.AI;
using Runtime.Managers;
using UnityEngine;

namespace Runtime.BehaviourTree 
{
    [System.Serializable]
    [Name("Root")]
    [Description("The root node of a behaviour tree. Links to the starting point of the tree.")]
    [DefaultExecutionOrder(6)]
    public class RootNode : Node 
    {
        [SerializeReference] public string name;
        [SerializeReference] public bool debug;
        [SerializeReference] [HideInInspector] public Node child;
    
        protected override void OnStart()
        {
            state = State.Running;
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            var data = new DebugData()
            {
                title = name,
                keys = GetKeyValueList()
            };
            if(debug) GameManager.Instance.AIManager.AddData(data);
            return child?.Update() ?? State.Failure;
        }

        protected override void OnReset() { child?.Reset(); }

        private List<string> GetKeyValueList()
        {
            return (from blackboardValue in blackboard.keys where !IsCulled(blackboardValue.name) select blackboardValue.ToString()).ToList();
        }

        private string[] _culledKeys = {
            "Player",
            "Rooms",
            "AggroLevel",
            "MenaceState",
            "PreviousRoom"
        };
        
        private bool IsCulled(string instance)
        {
            return _culledKeys.Any(instance.Contains);
        }
    }
}