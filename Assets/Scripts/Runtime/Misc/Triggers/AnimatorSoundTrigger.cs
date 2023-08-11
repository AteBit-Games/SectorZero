/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections.Generic;
using Runtime.Managers;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.Misc.Triggers
{
    public class AnimatorSoundTrigger : MonoBehaviour
    {
        [SerializeField] private List<Sound> sounds;
        [SerializeField] private List<Sound> randomSounds;

        public void TriggerIndexedSound(int id)
        {
            if (id < 0 || id >= sounds.Count) Debug.LogError("Sound ID does not exist!");
            GameManager.Instance.SoundSystem.Play(sounds[id], transform.GetComponent<AudioSource>());
        }
        
        private void TriggerRandomSound()
        {
            var audioSource = transform.GetComponent<AudioSource>();
            GameManager.Instance.SoundSystem.Play(randomSounds[Random.Range(0, randomSounds.Count)], audioSource);
        }
    }
}
