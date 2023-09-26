/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using Runtime.AI;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.Misc
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(NoiseEmitter))]
    public class Alarm : MonoBehaviour, ISoundEntity
    {
        [SerializeField] private Sound sound;
        
        public AudioSource AudioSource { get; private set; }
        public Sound Sound => sound;
        
        private NoiseEmitter _noiseEmitter;
        
        public void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
            _noiseEmitter = GetComponent<NoiseEmitter>();
        }
        
        public void Trigger()
        {
            GameManager.Instance.SoundSystem.Play(sound, AudioSource);
            _noiseEmitter.EmitGlobal();
        }
    }
}