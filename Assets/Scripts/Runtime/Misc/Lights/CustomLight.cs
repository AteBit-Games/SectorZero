/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Runtime.Misc.Lights
{
    [DefaultExecutionOrder(5)]
    public class CustomLight : MonoBehaviour, IPowered, ISoundEntity
    {
        [SerializeField] private new Light2D light;

        [SerializeField] private bool flicker;
        [SerializeField] private float minOn = 3f;
        [SerializeField] private float maxOn = 6f;
        [SerializeField] private float minOff = 0.1f;
        [SerializeField] private float maxOff = 0.3f;

        [SerializeField] private Sound onSound;
        [SerializeField] private Sound loopSound;
        [SerializeField] private Sound offSound;
        
        private Coroutine _coroutine;
        private Animator _animator;
        private static readonly int On = Animator.StringToHash("On");
        
        //----- Interface Variables -----//
        public bool IsPowered { get; set; }

        public AudioSource AudioSource { get; private set; }
        public Sound Sound => loopSound;

        //============================== Unity Events ==============================

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            
            AudioSource = GetComponent<AudioSource>();
            GameManager.Instance.SoundSystem.SetupSound(AudioSource, loopSound);
        }
        
        //============================== Interface ==============================
        
        public void PowerOn(bool load)
        {
            light.enabled = true;
            IsPowered = true;
            AudioSource.Play();
            
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
            AudioSource.Stop();
            
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }
        
        //============================== Coroutines ==============================
        
        private IEnumerator FlickerOff()
        {
            light.enabled = true;
            _animator.SetBool(On, true);
            GameManager.Instance.SoundSystem.PlayOneShot(onSound, AudioSource);

            yield return new WaitForSeconds(Random.Range(minOn, maxOn));
            light.enabled = false;
            _animator.SetBool(On, false);
            GameManager.Instance.SoundSystem.PlayOneShot(offSound, AudioSource);
            if(minOff >= 0.5) yield return FadeOut();

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
