using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
        var raycastHit2D = Physics2D.Raycast(transform.position, new Vector3(0f, 0f, -1f), 1f);
        if (raycastHit2D.collider != null)
        {
            if (raycastHit2D.collider.CompareTag("Water"))
            {
                audioSource.PlayOneShot(waterFootsteps[Random.Range(0, waterFootsteps.Count)]);
            }
            else if (raycastHit2D.collider.CompareTag("Concrete"))
            {
                audioSource.PlayOneShot(concreteFootsteps[Random.Range(0, concreteFootsteps.Count)]);
            }
        }
        
        if(debug)
        {
            if (noiseEffect != null)
            {
                var newParticleSystem = Instantiate(noiseEffect, audioSource.transform.position, Quaternion.identity);
                var mainModule = newParticleSystem.main;
                mainModule.startSize = 10f;
                newParticleSystem.Play();
                Destroy(newParticleSystem.gameObject, 4f);
            }
        }
    }
}
