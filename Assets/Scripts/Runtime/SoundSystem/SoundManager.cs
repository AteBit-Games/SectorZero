/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.SoundSystem
{
	public class SoundManager : MonoBehaviour
	{
	    public static SoundManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        [SerializeField] private AudioSource musicAudioSource;
        [SerializeField] private AudioSource soundAudioSource;

        private readonly Dictionary<Sound, float> _soundTimers = new Dictionary<Sound, float>();

        public void Play(Sound sound)
        {
            if(sound.loop)
            {
                musicAudioSource.clip = sound.clip;
                musicAudioSource.volume = sound.volume;
                musicAudioSource.pitch = sound.pitch;
                musicAudioSource.Play();
            }
            else
            {
                soundAudioSource.PlayOneShot(sound.clip, sound.volume);
            }
        }

        private void Update()
        {
            foreach (var sound in _soundTimers.Keys)
            {
                _soundTimers[sound] -= Time.deltaTime;
                if (_soundTimers[sound] <= 0)
                {
                    _soundTimers.Remove(sound);
                    Stop(sound);
                }
            }
        }

        public void Stop(Sound sound)
        {
            if (sound.loop)
            {
                musicAudioSource.Stop();
            }
            else
            {
                soundAudioSource.Stop();
            }
        }

        public void Play(Sound sound, float duration)
        {
            Play(sound);
            _soundTimers[sound] = duration;
        }
    }
	
}
