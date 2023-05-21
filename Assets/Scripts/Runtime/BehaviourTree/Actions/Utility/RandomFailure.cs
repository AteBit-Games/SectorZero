using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.BehaviourTree.Actions.Utility 
{
    [Serializable]
    [Name("Random Failure")]
    [Category("Utility")]
    [Description("Randomly fails")]
    public class RandomFailure : ActionNode
    {
        [Range(0,1)]
        [Tooltip("Percentage chance of failure")] public float chanceOfFailure = 0.5f;

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate() 
        {
            return Random.value < chanceOfFailure ? State.Failure : State.Success;
        }

        protected override void OnReset() { }
    }
}