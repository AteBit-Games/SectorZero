using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.BehaviourTree.Composites 
{
    [Serializable]
    [Name("Probability Selector")]
    [Category("Composites")]
    [Description("Selects a child to execute based on its chance to be selected and returns Success if the child returns Success, otherwise picks another child.\nReturns Failure if all children return Failure, or a direct 'Failure Chance' is introduced.")]
    public class ProbabilitySelector : CompositeNode
    {
        [Tooltip("The weights of the children.")]
        public List<NodeProperty<float>> childWeights = new();
        [Tooltip("A chance for the node to fail immediately.")]
        public NodeProperty<float> failChance;
        
        private bool[] indexFailed;
        private float[] temporaryWeights;
        private float temporaryFailWeight;
        private float temporaryTotal;
        private float temporaryDice;
        
        protected override void OnStart() { }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            if (state == State.Inactive )
            {
                temporaryDice = Random.value;
                temporaryFailWeight = failChance.Value;
                temporaryTotal = temporaryFailWeight;
                
                for ( var i = 0; i < childWeights.Count; i++ ) 
                {
                    var childWeight = childWeights[i].Value;
                    temporaryTotal += childWeight;
                    temporaryWeights[i] = childWeight;
                }
            }
            
            var probability = temporaryFailWeight / temporaryTotal;
            if (temporaryDice < probability) return State.Failure;

            for (var i = 0; i < children.Count; i++)
            {
                if (indexFailed[i]) continue;
                
                probability = temporaryWeights[i] / temporaryTotal;
                if (temporaryDice <= probability)
                {
                    state = children[i].Update();
                    if (state is State.Success or State.Running) return state;
                }
                
                if (state == State.Failure)
                {
                    indexFailed[i] = true;
                    temporaryTotal -= temporaryWeights[i];
                    return State.Running;
                }
            }

            return State.Success;
        }

        protected override void OnReset()
        {
            temporaryWeights = new float[children.Count];
            indexFailed = new bool[children.Count];
        }
    }
}