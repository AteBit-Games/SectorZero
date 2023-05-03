using System.Collections.Generic;
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
    
        [Header("DEBUG")]
        [SerializeField] private bool debug;
        [SerializeField] private ParticleSystem noiseEffect;
    
        public void PlayFootstepSound()
        {
            audioSource.volume = 0.5f;
            audioSource.PlayOneShot(DetermineSound());
            
            if(debug) ShowDebugSound(4f);
        }

        public void PlaySneakSound()
        {
            audioSource.volume = 0.3f;
            audioSource.PlayOneShot(DetermineSound());
            
            if(debug) ShowDebugSound(2.5f);
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
                else if (raycastHit2D.collider.CompareTag("Concrete"))
                {
                    return concreteFootsteps[Random.Range(0, concreteFootsteps.Count)];
                }
            }

            return null;
        }

        private void ShowDebugSound(float range)
        {
            if(debug)
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
}
