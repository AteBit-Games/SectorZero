/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.Managers;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.SoundSystem
{
    public class AmbienceTrigger : MonoBehaviour
    {
        public Sound ambClip;
    
    
        public void TriggerAmbience(float fadeTime)
        {
            GameManager.Instance.SoundSystem.FadeToNextAmbience(ambClip, fadeTime);
        } 
    
        public void TriggerFadeOut(float fadeTime)
        {
            GameManager.Instance.SoundSystem.FadeOutAmbience(fadeTime);
        }
    
        public void TriggerSilence()
        {
            GameManager.Instance.SoundSystem.SilenceAmbience();
        }
    }
}
