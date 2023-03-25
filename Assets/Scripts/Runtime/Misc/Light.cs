
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Runtime.Misc
{
    public class Light : MonoBehaviour
    {
        [SerializeField] private bool flicker;
        [SerializeField] private float minSpeed = 0.1f;
        [SerializeField] private float maxSpeed = 0.5f;
        
        private float _timer;
        private AudioSource _audioSource;
        private Light2D _light;
        private Animator _animator;
        

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _light = GetComponent<Light2D>();
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
                _animator.SetBool("On", true);
                _timer -= Time.deltaTime;
            }
            
            if(_timer <= 0)
            {
                _audioSource.mute = true;
                _animator.SetBool("On", false);
                _timer = UnityEngine.Random.Range(minSpeed, maxSpeed);
                _light.enabled = !_light.enabled;
            }
        }
    }
}
