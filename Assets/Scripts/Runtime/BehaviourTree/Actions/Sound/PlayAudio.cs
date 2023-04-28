using System;
using Runtime.Managers;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.Sound 
{
    [Serializable]
    [Category("Sound")]
    [Name("Play Audio")]
    [Description("Play audio clip")]
    public class PlayAudio : ActionNode
    {
        public SoundSystem.ScriptableObjects.Sound sound;
        public bool waitActionFinish;
        
        private float _startTime;

        protected override void OnStart()
        {
            GameManager.Instance.SoundSystem.Play(sound, context.transform);
            _startTime = Time.time;
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate()
        {
            if(!waitActionFinish) return State.Success;
            
            return Time.time - _startTime >= sound.clip.length ? State.Success : State.Running;
        }
        
        protected override void OnReset() { }
    }
}
