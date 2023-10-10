/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
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
        
        [SerializeField] private Sound alarmSound;
        public Sound InteractSound => alarmSound;
        
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
            StartCoroutine(AlarmCoroutine());
            
            GameManager.Instance.SoundSystem.Play(alarmSound, _alarmSource);
            player.GetComponent<PlayerInteraction>().RemoveInteractable(gameObject);
            return true;
        }

        private IEnumerator AlarmCoroutine()
        {
            yield return new WaitForSeconds(alarmDelay);
            _noiseEmitter.EmitGlobal();
            yield return new WaitForSeconds(alarmSound.clip.length+1f);
            _isTriggered = false;
        }

        public void OnInteractFailed(GameObject player)
        {
           
        }

        public bool CanInteract()
        {
            return !_isTriggered;
        }
    }
}