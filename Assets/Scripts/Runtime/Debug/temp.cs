/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;


public class Temp : MonoBehaviour
{
    [SerializeField] private Sound voices;
    [SerializeField] private GameObject voidMask;
    
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            _audioSource.clip = voices.clip;
            _audioSource.loop = false;
            _audioSource.Play();
            voidMask.SetActive(true);
        }
    }
}
