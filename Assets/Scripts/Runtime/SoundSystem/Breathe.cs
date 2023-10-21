/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections;
using System.Collections.Generic;
using Runtime.Managers;
using UnityEngine;

namespace Runtime.SoundSystem
{
    [DefaultExecutionOrder(6)]
	public class Breathe : MonoBehaviour
	{
        [SerializeField] public List<Sound> breathSounds;
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            GameManager.Instance.SoundSystem.SetupSound(_audioSource, breathSounds[0]);
        }
        
        private void Start()
        {
            StartCoroutine(Breath());
        }

        private IEnumerator Breath()
        {
            var clip = breathSounds[Random.Range(0, breathSounds.Count)];
            GameManager.Instance.SoundSystem.Play(clip, _audioSource);
            
            yield return new WaitForSeconds(clip.clip.length);
            StartCoroutine(Breath());
        }
    }
}
