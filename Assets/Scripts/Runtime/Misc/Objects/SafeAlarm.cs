/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
using Runtime.Managers;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.Misc.Objects
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(NoiseEmitter))]
    public class SafeAlarm : MonoBehaviour, ISoundEntity
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Sound sound;
        
        public AudioSource AudioSource { get; private set; }
        public Sound Sound => sound;
        
        private NoiseEmitter _noiseEmitter;
        private static readonly int Alarm = Animator.StringToHash("alarm");

        public void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
            _noiseEmitter = GetComponent<NoiseEmitter>();
        }
        
        public void Trigger()
        {
            GameManager.Instance.SoundSystem.Play(sound, AudioSource);
            _noiseEmitter.EmitGlobal();
            animator.SetBool(Alarm, true);
            StartCoroutine(StopAlarm());
        }

        private IEnumerator StopAlarm()
        {
            yield return new WaitForSeconds(sound.clip.length);
            animator.SetBool(Alarm, false);
        }
    }
}