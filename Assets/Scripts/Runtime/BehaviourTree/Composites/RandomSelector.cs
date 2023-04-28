using System;
using Random = UnityEngine.Random;

namespace Runtime.BehaviourTree.Composites 
{
    [Serializable]
    [Name("Random Selector")]
    [Category("Composites")]
    [Description("Selects a child to execute at random.")]
    public class RandomSelector : CompositeNode 
    {
        protected int current;

        protected override void OnStart() 
        {
            current = Random.Range(0, children.Count);
        }

        protected override void OnStop() { }

        protected override State OnUpdate() 
        {
            var child = children[current];
            return child.Update();
        }

        protected override void OnReset()
        {
            current = Random.Range(0, children.Count);
        }
    }
}