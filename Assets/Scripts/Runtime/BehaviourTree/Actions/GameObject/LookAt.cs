using System;
using Tweens;
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
            var rotationTween = new EulerAnglesTween
            {
                to = Quaternion.LookRotation(Vector3.forward, direction).eulerAngles,
                duration = lookTime.Value-1,
                easeType = EaseType.SineInOut,
            };
            context.agent.transform.gameObject.AddTween(rotationTween);
            
            var directionNormalized = direction.normalized;
            var lookDirectionTween = new Vector2Tween
            {
                to = directionNormalized,
                duration = lookTime.Value-1,
                easeType = EaseType.SineInOut,
                from = context.owner.GetLookDirection(),
                onUpdate = (_, value) => {
                    context.owner.SetLookDirection(value);
                }
            };
            context.owner.gameObject.AddTween(lookDirectionTween);
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
