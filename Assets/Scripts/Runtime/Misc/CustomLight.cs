/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System.Collections;
using Runtime.InteractionSystem.Interfaces;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Runtime.Misc
{
    [DefaultExecutionOrder(18)]
    public class CustomLight : MonoBehaviour, IPowered, ISoundEntity
    {
        [SerializeField] private new Light2D light;
        [SerializeField] private bool startPowered;

        [SerializeField] private bool flicker;
        [SerializeField] private float minOn = 3f;
        [SerializeField] private float maxOn = 6f;
        [SerializeField] private float minOff = 0.1f;
        [SerializeField] private float maxOff = 0.3f;

        [SerializeField] private Sound onSound;
        [SerializeField] private Sound loopSound;
        [SerializeField] private Sound offSound;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float volumeScale;

        [HideInInspector] public bool isCulled;
        
        private Coroutine _coroutine;
        private Animator _animator;
        private static readonly int On = Animator.StringToHash("On");
        
        //----- Interface Variables -----//
        public bool IsPowered { get; set; }

        public AudioSource AudioSource { get; set; }
        public Sound Sound => loopSound;

        public float Volume { get; set; }
        public float VolumeScale
        {
            get => volumeScale;
            set => volumeScale = value;
        }
        
        //============================== Unity Events ==============================

        private void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            AudioSource.clip = loopSound.clip;
            AudioSource.Play();
            if (startPowered) PowerOn();
            else PowerOff();
        }

        //============================== Interface ==============================
        
        public void PowerOn()
        {
            light.enabled = true;
            IsPowered = true;
            AudioSource.mute = false;
            if (flicker)
            {
                if (_coroutine != null) StopCoroutine(_coroutine);
                _coroutine = StartCoroutine(FlickerOff());
            }
        }

        public void PowerOff()
        {
            light.enabled = false;
            IsPowered = false;
            if(AudioSource != null) AudioSource.mute = true;
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }
        
        public void UnCull()
        {
            isCulled = false;
            if (IsPowered) PowerOn();
            else PowerOff();
        }
        
        public void Cull()
        {
            isCulled = true;
            PowerOff();
        }
        
        //============================== Coroutines ==============================
        
        private IEnumerator FlickerOff()
        {
            light.enabled = true;
            _animator.SetBool(On, true);
            AudioSource.PlayOneShot(onSound.clip);

            yield return new WaitForSeconds(Random.Range(minOn, maxOn));
            light.enabled = false;
            _animator.SetBool(On, false);
            AudioSource.PlayOneShot(offSound.clip);
            if(minOff >= 0.5)
                yield return FadeOut();

            yield return new WaitForSeconds(Random.Range(minOff, maxOff));
            AudioSource.Play();
            _coroutine = StartCoroutine(FlickerOff());
        }

        private IEnumerator FadeOut()
        {
            var initialVolume = AudioSource.volume;
            while (AudioSource.volume > 0)
            {
                AudioSource.volume -= Time.deltaTime / 2f;
                yield return null;
            }

            AudioSource.Stop();
            AudioSource.volume = initialVolume;
        }
    }
}
