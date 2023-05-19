using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Conditions.GameObject 
{
    [Serializable]
    [Name("Check Distance")]
    [Category("GameObject")]
    [Description("Check the distance between the context and the target")]
    public class CheckDistance : ConditionNode
    {
        public NodeProperty<float> distance;
        public NodeProperty<UnityEngine.GameObject> target;

        protected override void OnStart() { }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate()
        {
            return Vector2.Distance(target.Value.transform.position, context.agent.transform.position) < distance.Value ? State.Success : State.Failure;
        }
        
        protected override void OnReset() { }
    }
}
