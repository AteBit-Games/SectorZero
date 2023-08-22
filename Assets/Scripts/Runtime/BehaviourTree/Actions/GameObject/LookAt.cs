using System;
using ElRaccoone.Tweens;
using ElRaccoone.Tweens.Core;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.GameObject 
{
    [Serializable]
    [Name("Look At")]
    [Category("GameObject")]
    [Description("The agent looks at a target for a set duration.")]
    public class LookAt : ActionNode
    {
        public NodeProperty<Vector2> target = new(){Value = Vector2.zero};
        public NodeProperty<float> lookTime = new(){Value = 2f};
        
        private float _lookTime;

        protected override void OnStart()
        {
            _lookTime = Time.time;
            var direction = target.Value - (Vector2)context.agent.transform.position;
            context.agent.transform.TweenRotation(Quaternion.LookRotation(Vector3.forward, direction).eulerAngles, lookTime.Value - 1).SetEaseSineOut();
            
            var directionNormalized = direction.normalized;
            context.owner.TweenValueVector2(directionNormalized, lookTime.Value - 1, value =>
            {
                context.owner.SetLookDirection(value);
            }).SetFrom(context.owner.GetLookDirection()).SetEaseSineOut();
            //context.owner.SetLookDirection(directionNormalized);
        }

        protected override void OnStop() { }
    
        protected override State OnUpdate()
        {
            return Time.time - _lookTime >= lookTime.Value ? State.Success : State.Running;
        }
        
        protected override void OnReset() { }

        public override void OnDrawGizmos() { }
    }
}
