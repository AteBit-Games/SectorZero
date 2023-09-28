/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
using System.Collections.Generic;
using Runtime.InteractionSystem;
using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.Misc.Objects
{
    [RequireComponent(typeof(NoiseEmitter))]
    public class AlarmClock: MonoBehaviour, IInteractable
    {
        [SerializeField] private float alarmDelay = 4f;
        
        [SerializeField] private Sound failSound;
        [SerializeField] private Sound alarmSound;
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        private AudioSource _alarmSource;
        private NoiseEmitter _noiseEmitter;
        private bool _isTriggered;
        
        public void Awake()
        {
            _noiseEmitter = GetComponent<NoiseEmitter>();
            _alarmSource = gameObject.GetComponent<AudioSource>();
        }
        
        public bool OnInteract(GameObject player)
        {
            _isTriggered = true;
            GameManager.Instance.SoundSystem.Play(interactSound, _alarmSource);
            
            StartCoroutine(AlarmCoroutine());
            player.GetComponent<PlayerInteraction>().RemoveInteractable(gameObject);
            
            return true;
        }

        private IEnumerator AlarmCoroutine()
        {
            yield return new WaitForSeconds(alarmDelay);
            GameManager.Instance.SoundSystem.Play(alarmSound, _alarmSource);
            _noiseEmitter.EmitLocal();
            yield return new WaitForSeconds(alarmSound.clip.length+1f);
            _isTriggered = false;
        }

        public void OnInteractFailed(GameObject player)
        {
            GameManager.Instance.SoundSystem.Play(failSound, _alarmSource);
        }

        public bool CanInteract()
        {
            return !_isTriggered;
        }
    }
}