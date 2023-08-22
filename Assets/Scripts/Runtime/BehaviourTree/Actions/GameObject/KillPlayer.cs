using System;
using Runtime.Player;
using Runtime.Utils;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.GameObject 
{
    [Serializable]
    [Name("Kill Player")]
    [Category("GameObject")]
    [Description("Kill the player")]
    public class KillPlayer : ActionNode
    {
        public NodeProperty<UnityEngine.GameObject> player;
        public NodeProperty<AnimationClip> northKill;
        public NodeProperty<AnimationClip> eastKill;
        public NodeProperty<AnimationClip> westKill;
        public NodeProperty<AnimationClip> southKill;
        public bool waitUntilFinish = true;
        
        private float _startTime;
        private AnimationClip animationClip;

        protected override void OnStart()
        {
            context.agent.isStopped = true;
            context.agent.velocity = Vector3.zero;
            
            var direction = context.agent.transform.position - player.Value.transform.position;
            // var angle = Vector3.SignedAngle(direction, Vector3.up, Vector3.forward);
            //
            // //for now only west and east
            // animationClip = angle switch
            // {
            //     < 45f and > -45f => eastKill,
            //     < 135f and > 45f => northKill,
            //     < -135f or > 135f => westKill,
            //     < -45f and > -135f => southKill,
            //     _ => throw new ArgumentOutOfRangeException()
            // };
            
            var perp = Vector3.Cross(context.transform.forward, direction);
            var dir = Vector3.Dot(perp, player.Value.transform.up);

            animationClip = dir switch
            {
                <= 0f => eastKill.Value,
                > 0f => westKill.Value,
                _ => eastKill.Value
            };
            
            context.animator.Play(animationClip.name);
            _startTime = Time.time;

            if(player.Value.TryGetComponent(out PlayerController playerController))
            {
                playerController.Die();
            }
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            if (waitUntilFinish)
            {
                return Time.time - _startTime < animationClip.length ? State.Running : State.Success;
            }
            else
            {
                return State.Success;
            }
        }
        
        protected override void OnReset() { }
    }
}
