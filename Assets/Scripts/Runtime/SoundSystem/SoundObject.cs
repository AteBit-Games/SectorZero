/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using Runtime.Managers;
using UnityEngine;

namespace Runtime.SoundSystem
{
    [RequireComponent(typeof(AudioSource))]
    [DefaultExecutionOrder(5)]
    public class SoundObject : MonoBehaviour, ISoundEntity
    {
        [SerializeField] private Sound sound;
        [SerializeField] private bool playOnStart = true;

        public AudioSource AudioSource { get; private set; }
        public Sound Sound => sound;

        private void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
            GameManager.Instance.SoundSystem.SetupSound(AudioSource, sound);
        }

        public void Start()
        {
            if (playOnStart) AudioSource.Play();
        }
    }
}
