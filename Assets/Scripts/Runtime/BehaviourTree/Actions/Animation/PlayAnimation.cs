using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.Animation 
{
    [Serializable]
    [Name("Play Animation")]
    [Category("Animation")]
    [Description("Plays an animation clip")]
    public class PlayAnimation : ActionNode
    {
        public AnimationClip animationClip;
        public bool waitUntilFinish = true;
        
        private float _startTime;
        
        protected override void OnStart()
        {
            if (animationClip == null)
            {
                throw new NullReferenceException("Animation clip is null");
            }
            
            context.animator.Play(animationClip.name);
            _startTime = Time.time;
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
