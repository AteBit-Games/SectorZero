/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections;
using Runtime.InteractionSystem;
using Runtime.InteractionSystem.Interfaces;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Runtime.Misc
{
    public class CustomLight : MonoBehaviour, IPowered
    {
        [SerializeField] private bool flicker;
        [SerializeField] private float minWait = 3f;
        [SerializeField] private float maxWait = 6f;
        [SerializeField] private new Light2D light;
        [SerializeField] private bool startPowered;
        
        private Coroutine _coroutine;
        private AudioSource _audioSource;
        private Animator _animator;
        private static readonly int On = Animator.StringToHash("On");

        private void Awake()
        {
            if(startPowered) PowerOn();
            else PowerOff();
            
            _audioSource = GetComponent<AudioSource>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if(flicker) _coroutine = StartCoroutine(FlickerOff());
        }

        public bool IsPowered { get; set; }
        public void PowerOn()
        {
            light.enabled = true;
            IsPowered = true;
            if(flicker) _coroutine = StartCoroutine(FlickerOff());
        }

        public void PowerOff()
        {
            light.enabled = false;
            IsPowered = false;
            if(_coroutine != null) StopCoroutine(_coroutine);
        }
        
        private IEnumerator FlickerOff() 
        {
            light.enabled = true;
            _audioSource.mute = false;
            _animator.SetBool(On, true);
            
            yield return new WaitForSeconds(Random.Range(minWait, maxWait));
            light.enabled = false;
            _audioSource.mute = true;
            _animator.SetBool(On, false);
            
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
            
            _coroutine = StartCoroutine(FlickerOff());
        }
    }
}
