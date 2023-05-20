/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System.Collections;
using Runtime.InteractionSystem.Interfaces;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Runtime.Misc
{
    public class CustomLight : MonoBehaviour, IPowered
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

        private Coroutine _coroutine;
        private Animator _animator;
        private static readonly int On = Animator.StringToHash("On");
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _audioSource.clip = loopSound.clip;
            _audioSource.Play();
            if(startPowered) PowerOn();
            else PowerOff();
        }

        public bool IsPowered { get; set; }
        public void PowerOn()
        {
            light.enabled = true;
            IsPowered = true;
            _audioSource.mute = false;
            if(flicker) _coroutine = StartCoroutine(FlickerOff());
        }

        public void PowerOff()
        {
            light.enabled = false;
            IsPowered = false;
            _audioSource.mute = true;
            if(_coroutine != null) StopCoroutine(_coroutine);
        }
        
        private IEnumerator FlickerOff() 
        {
            light.enabled = true;
            _animator.SetBool(On, true);
            _audioSource.PlayOneShot(onSound.clip);
            
            yield return new WaitForSeconds(Random.Range(minOn, maxOn));
            light.enabled = false;
            _animator.SetBool(On, false);
            _audioSource.PlayOneShot(offSound.clip);

            yield return new WaitForSeconds(Random.Range(minOff, maxOff));
            _coroutine = StartCoroutine(FlickerOff());
        }
    }
}
