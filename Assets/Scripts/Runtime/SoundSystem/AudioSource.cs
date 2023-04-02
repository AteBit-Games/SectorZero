// Copyright (c) 2023 AteBit Games

using System;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.SoundSystem
{
    public abstract class CustomAudioSource : MonoBehaviour
    {
        [SerializeField] [HideInInspector] protected float volume = 1f;
        [SerializeField] [HideInInspector] protected float pitch = 1f;
        private bool _playedFromStart;
        
        public AudioCue audioCue;
        public bool loop;
        public bool playOnStart = true;

        public float Volume
        {
            get => volume;
            set 
            {
                if(Math.Abs(volume - value) > 0.0001f)
                {
                    volume = Mathf.Clamp01(value);
                    OnVolumePitchChanged();
                }
            }
        }

        public float Pitch
        {
            get => pitch;
            set 
            {
                if(Math.Abs(pitch - value) > 0.0001f)
                {
                    pitch = Mathf.Clamp(value, 0f, 2f);
                    OnVolumePitchChanged();
                }
            }
        }
        
        public abstract bool isPlaying { get; }
        public abstract void Play();
        public abstract void Stop(bool stopImmediately);

        /// Hook to play sources from animation events.
        public void PlayEvent()
        {
            Play();
        }

        public void StopEvent(bool stopImmediately)
        {
            Stop(stopImmediately);
        }
        
        private void Start()
        {
            if(playOnStart && Application.isPlaying)
            {
                Play();
                _playedFromStart = true;
            }
        }

        private void OnEnable()
        {
            if(playOnStart && _playedFromStart && Application.isPlaying)
            {
                Play();
            }
        }

        protected virtual void OnDisable()
        {
            Stop(true);
        }
        
        protected abstract void OnVolumePitchChanged();
    }
}