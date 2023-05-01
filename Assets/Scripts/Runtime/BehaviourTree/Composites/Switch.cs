using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.BehaviourTree.Composites 
{
    [Serializable]
    [Name("Switch")]
    [Category("Composites")]
    [Description("Executes one child based on the provided int and returns its status.")]
    public class Switch : CompositeNode
    {
        [Space(10)]
        [Header("PROPERTIES")]
        public NodeProperty<int> index;
        public bool interruptable = true;
        
        private int _currentIndex;

        protected override void OnStart()
        {
            _currentIndex = 0;
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (interruptable) 
            {
                int nextIndex = index.Value;
                if (nextIndex != _currentIndex) {
                    children[_currentIndex].Abort();
                }
                _currentIndex = nextIndex;
            }

            return _currentIndex < children.Count ? children[_currentIndex].Update() : State.Failure;
        }

        protected override void OnReset()
        {
            _currentIndex = 0;
        }
    }
}

