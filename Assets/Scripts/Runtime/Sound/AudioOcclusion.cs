
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace Runtime.Sound
{
    public class AudioOcclusion : MonoBehaviour
    {
        public float maxDistance = 10f; 
        public LayerMask occlusionMask;
        public Transform audioListener;
        public AudioMixerGroup defaultMixerGroup;
        public AudioMixerGroup muffledMixerGroup;

        private RaycastHit2D[] _hits; 
        private float[] _distances; 
        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            _audioSource.volume = 1f;
            _audioSource.outputAudioMixerGroup = defaultMixerGroup;

            Vector2 direction = audioListener.position - transform.position;
            _hits = Physics2D.RaycastAll(transform.position, direction.normalized, maxDistance, occlusionMask.value);
            
            if(_hits.Length == 0) return;
            
            _audioSource.outputAudioMixerGroup = muffledMixerGroup;

            _distances = new float[_hits.Length];
            for (var i = 0; i < _hits.Length; i++)
            {
                _distances[i] = Vector2.Distance(transform.position, _hits[i].point);
            }
            
            var occlusionAmount = _distances.Aggregate(1f, (current, item) => current * Mathf.Clamp01(1f - item / maxDistance));
            _audioSource.volume = 1f - occlusionAmount;
        }
    }
}
