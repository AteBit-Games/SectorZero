/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.BehaviourTree.Actions.Utility 
{
    [Serializable]
    [Name("Random Failure")]
    [Category("Utility")]
    [Description("Fails the execution of the node based on a percentage chance")]
    public class RandomFailure : ActionNode
    {
        [Tooltip("Percentage chance of failure")] public NodeProperty<float> chanceOfFailure = new(){Value = 0.5f};

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate() 
        {
            return Random.value < chanceOfFailure.Value ? State.Failure : State.Success;
        }

        protected override void OnReset() { }
    }
}