using System;
using Runtime.Managers;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.Sound 
{
    [Serializable]
    [Category("Sound")]
    [Name("Play Ambience")]
    [Description("Play ambience")]
    public class PlayAmbience : ActionNode
    {
        public SoundSystem.ScriptableObjects.Sound sound;
        public NodeProperty<float> fadeTime = new(){Value = 1f};
        
        protected override void OnStart()
        {
            GameManager.Instance.SoundSystem.FadeToNextAmbience(sound, fadeTime.Value);
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
