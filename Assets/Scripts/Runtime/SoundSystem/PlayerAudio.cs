
using System.Collections.Generic;
using Runtime.AI;
using Runtime.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.SoundSystem
{
    public class PlayerAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<AudioClip> concreteFootsteps;
        [SerializeField] private List<AudioClip> waterFootsteps;
        [SerializeField] private float joggingRange = 18f;
        [SerializeField] private float sneakingRange = 6f;
        [SerializeField] private bool debug;

        private NoiseEmitter _noiseEmitter;

        //========================= Unity Events =========================//
        
        private void Awake()
        {
            _noiseEmitter = audioSource.GetComponent<NoiseEmitter>();
        }
        
        //========================= Public Methods =========================//

        public void PlayFootstepSound()
        {
            audioSource.volume = 0.12f * GameManager.Instance.SoundSystem.SfxVolume();
            audioSource.PlayOneShot(DetermineSound());
            _noiseEmitter.Radius = joggingRange;
            _noiseEmitter.EmitLocal();
        }

        public void PlaySneakSound()
        {
            audioSource.volume = 0.06f * GameManager.Instance.SoundSystem.SfxVolume();
            audioSource.PlayOneShot(DetermineSound());
            _noiseEmitter.Radius = sneakingRange;
            _noiseEmitter.EmitLocal();
        }
        
        //========================= Private Methods =========================//

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
        
        //========================= Unity Gizmos =========================//

        private void OnDrawGizmos()
        {
            if (debug)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, joggingRange);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, sneakingRange);
            }
        }
    }
}
