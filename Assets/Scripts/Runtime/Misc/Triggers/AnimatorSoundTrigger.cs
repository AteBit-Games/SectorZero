/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections.Generic;
using Runtime.Managers;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.Misc.Triggers
{
    public class AnimatorSoundTrigger : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<Sound> indexedSounds;
        [SerializeField] private List<Sound> randomSounds;
        
        public void TriggerIndexedSound(int id)
        {
            if (id < 0 || id >= indexedSounds.Count) Debug.LogError("Sound ID does not exist!");
            
            var source = audioSource ? audioSource : transform.GetComponent<AudioSource>();
            GameManager.Instance.SoundSystem.Play(indexedSounds[id], source);
        }
        
        private void TriggerRandomSound()
        {
            var source = audioSource ? audioSource : transform.GetComponent<AudioSource>();
            GameManager.Instance.SoundSystem.Play(randomSounds[Random.Range(0, randomSounds.Count)], source);
        }
    }
}
