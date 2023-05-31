using System;
using System.Collections.Generic;
using Runtime.AI;
using Runtime.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.SoundSystem
{
    public class PlayerAudio : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private AudioSource audioSource;
    
        [Header("SOUNDS")]
        [SerializeField] private List<AudioClip> concreteFootsteps;
        [SerializeField] private List<AudioClip> waterFootsteps;

        [Header("SOUND PROPERTIES")] 
        [SerializeField] private float joggingRange = 18f;
        [SerializeField] private float sneakingRange = 6f;
    
        [Header("DEBUG")]
        [SerializeField] private bool debug;
        [SerializeField] private ParticleSystem noiseEffect;
        
        private NoiseEmitter _noiseEmitter;

        private void Awake()
        {
            _noiseEmitter = audioSource.GetComponent<NoiseEmitter>();
        }

        public void PlayFootstepSound()
        {
            audioSource.volume = 0.12f * GameManager.Instance.SoundSystem.SfxVolume();
            audioSource.PlayOneShot(DetermineSound());
            _noiseEmitter.Radius = joggingRange;
            _noiseEmitter.EmitLocal();
            
            if(debug) ShowDebugSound(joggingRange);
        }

        public void PlaySneakSound()
        {
            audioSource.volume = 0.06f * GameManager.Instance.SoundSystem.SfxVolume();
            audioSource.PlayOneShot(DetermineSound());
            _noiseEmitter.Radius = sneakingRange;
            _noiseEmitter.EmitLocal();
            
            if(debug) ShowDebugSound(sneakingRange);
        }

        private AudioClip DetermineSound()
        {
            var raycastHit2D = Physics2D.Raycast(transform.position, new Vector3(0f, 0f, -1f), 1f);
            if (raycastHit2D.collider != null)
            {
                if (raycastHit2D.collider.CompareTag("Water"))
                {
                    return waterFootsteps[Random.Range(0, waterFootsteps.Count)];
                }
                
                if (raycastHit2D.collider.CompareTag("Concrete"))
                {
                    return concreteFootsteps[Random.Range(0, concreteFootsteps.Count)];
                }
            }

            return concreteFootsteps[0];
        }

        private void ShowDebugSound(float range)
        {
            if (noiseEffect != null)
            {
                var newParticleSystem = Instantiate(noiseEffect, audioSource.transform.position, Quaternion.Euler(90, 0,0 ));
                newParticleSystem.transform.localScale = new Vector3(range, range, range);
                newParticleSystem.Play();
                Destroy(newParticleSystem.gameObject, 2f);
            }
        }
    }
}
