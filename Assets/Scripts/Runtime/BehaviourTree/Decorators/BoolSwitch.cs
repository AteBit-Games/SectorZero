/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Decorators 
{
    [Serializable]
    [Name("Boolean Watch")]
    [Category("Decorators")]
    [Description("Restart execution if the provided bool changes.")]
    public class BoolWatch : DecoratorNode
    {
        [Space(10)]
        [Header("PROPERTIES")]
        public NodeProperty<bool> valueToWatch;
        
        private bool _currentState;

        protected override void OnStart()
        {
            _currentState = false;
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            var nextState = valueToWatch.Value;
            if (nextState != _currentState) {
                child.Abort();
            }
            _currentState = nextState;
            
            return child.Update();
        }

        protected override void OnReset()
        {
            _currentState = false;
        }
    }
}

