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
    public class AnimatorSoundTriggerTemp : MonoBehaviour
    {
        [SerializeField] private List<Sound> indexedSounds;
        [SerializeField] private List<Sound> randomSounds;

        public void TriggerIndexedSound(int id)
        {
            if (id < 0 || id >= indexedSounds.Count) Debug.LogError("Sound ID does not exist!");
            var audioSource = transform.GetComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = indexedSounds[id].mixerGroup;
            audioSource.volume = indexedSounds[id].volumeScale;
            audioSource.PlayOneShot((indexedSounds[id].clip));
        }
        
        private void TriggerRandomSound()
        {
            var audioSource = transform.GetComponent<AudioSource>();
            GameManager.Instance.SoundSystem.Play(randomSounds[Random.Range(0, randomSounds.Count)], audioSource);
        }
    }
}
