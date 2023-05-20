/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.InteractionSystem;
using Runtime.InteractionSystem.Interfaces;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Runtime.Misc
{
    public class CustomLight : MonoBehaviour, IPowered
    {
        [SerializeField] private bool flicker;
        [SerializeField] private float minWait = 3f;
        [SerializeField] private float maxWait = 6f;
        [SerializeField] private new Light2D light;
        [SerializeField] private bool startPowered;
        
        private float _timer;
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

        private void Update()
        {
            if(flicker) Flicker();
        }

        private void Flicker()
        {
            if (_timer > 0)
            {
                _audioSource.mute = false;
                _animator.SetBool(On, true);
                _timer -= Time.deltaTime;
            }
            
            if(_timer <= 0)
            {
                _audioSource.mute = true;
                _animator.SetBool(On, false);
                _timer = Random.Range(minSpeed, maxSpeed);
                light.enabled = !light.enabled;
            }
        }

        public bool IsPowered { get; set; }
        public void PowerOn()
        {
            light.enabled = true;
            IsPowered = true;
        }

        public void PowerOff()
        {
            light.enabled = false;
            IsPowered = false;
        }
    }
}
