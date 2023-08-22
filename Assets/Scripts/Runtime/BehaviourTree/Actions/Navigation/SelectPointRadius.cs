using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.Navigation 
{
    [Serializable]
    [Name("Select Point Radius")]
    [Category("Navigation")]
    [Description("Selects a random point within a radius")]
    public class SelectPointRadius : ActionNode
    {
        public NodeProperty<float> radius;
        public NodeProperty<Vector2> origin;
        public NodeProperty<Vector2> outTarget;

        protected override void OnStart()
        {
            var randomPoint = UnityEngine.Random.insideUnitCircle * radius.Value;
            outTarget.Value = origin.Value + randomPoint;
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
