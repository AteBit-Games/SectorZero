/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections.Generic;
using Runtime.SoundSystem.ScriptableObjects;
using Runtime.Utils;
using UnityEngine;

namespace Runtime.Managers
{
    public class AmbienceManager : MonoBehaviour
    {
        [SerializeField] public List<AudioSource> audioSources;

        private int _currentAmbIndex;
        private readonly string[] _ambMixerNames = {"mainAmbA", "mainAmbB"};
        private bool _isBusy;

        private void Awake()
        {
            _currentAmbIndex = 0;
            _isBusy = false;
        }
        
        private void Start()
        {
            audioSources[0].Play();
            audioSources[1].Play();
        }
        
        public void FadeIn(float fadeTime = 1.0f)
        {
            StartCoroutine(SoundUtils.StartFade(GameManager.Instance.SoundSystem.mainMixer, _ambMixerNames[_currentAmbIndex], fadeTime, 0.0f));
        }
        
        public void FadeOut(float fadeTime = 1.0f)
        {
            StartCoroutine(SoundUtils.StartFade(GameManager.Instance.SoundSystem.mainMixer, _ambMixerNames[_currentAmbIndex], fadeTime, -80.0f));
        }

        public void SilenceAmbience()
        {
            GameManager.Instance.SoundSystem.mainMixer.SetFloat(_ambMixerNames[_currentAmbIndex], -80.0f);
        }

        public void FadeToNext(Sound nextAmb, float fadeTime = 1.0f)
        {
            audioSources[1 - _currentAmbIndex].clip = nextAmb.clip;
            audioSources[1 - _currentAmbIndex].volume = nextAmb.volume;
            audioSources[1 - _currentAmbIndex].loop = nextAmb.loop;
            audioSources[1 - _currentAmbIndex].Play();
            StartCoroutine(SoundUtils.StartFade(GameManager.Instance.SoundSystem.mainMixer, _ambMixerNames[_currentAmbIndex], fadeTime, -80.0f));
            StartCoroutine(SoundUtils.StartFade(GameManager.Instance.SoundSystem.mainMixer, _ambMixerNames[1 - (_currentAmbIndex)], fadeTime, 1.0f));
            _currentAmbIndex = 1 - _currentAmbIndex;
        }

        public void FastSwap()
        {
            _currentAmbIndex = 1 - _currentAmbIndex;
            audioSources[_currentAmbIndex].Play();
            audioSources[1 - _currentAmbIndex].Stop();
        }

        public void SetClip(Sound sound)
        {
            audioSources[_currentAmbIndex].clip = sound.clip;
        }
        
        public void SetNextClip(Sound sound)
        {
            audioSources[1 - _currentAmbIndex].clip = sound.clip;
        }

        public void PlaySting(Sound sound)
        {
            if(_isBusy) return;
            _isBusy = true;
            audioSources[2].PlayOneShot(sound.clip);
            Invoke(nameof(StingReady), sound.clip.length);
        }
        
        public void StingReady()
        {
            _isBusy = false;
        }

        // public void Play()
        // {
        //     audioSources[_currentAmbIndex].Play();
        // }
        //
        // public void Stop()
        // {
        //     audioSources[_currentAmbIndex].Stop();
        // }
        //
        // public void Mute()
        // {
        //     audioSources[_currentAmbIndex].mute = true;
        // }
        //
        // public void Unmute()
        // {
        //     audioSources[_currentAmbIndex].mute = false;
        // }
    }
}