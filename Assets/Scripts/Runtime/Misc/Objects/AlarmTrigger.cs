/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using System.Collections;
using Runtime.InteractionSystem;
using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.Misc.Objects
{
    public class AlarmTrigger : MonoBehaviour, IInteractable
    {
        [SerializeField] private Alarm alarm;
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        private Animator _animator;
        private bool _isTriggered;
        
        private static readonly int Triggered = Animator.StringToHash("triggered");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public bool OnInteract(GameObject player)
        {
            _animator.SetBool(Triggered, true);
            _isTriggered = true;
            GameManager.Instance.SoundSystem.Play(interactSound);
            
            player.GetComponent<PlayerInteraction>().RemoveInteractable(gameObject);
            StartCoroutine(ResetAlarm());
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            throw new NotImplementedException();
        }

        public bool CanInteract()
        {
            return !_isTriggered;
        }

        private IEnumerator ResetAlarm()
        {
            yield return new WaitForSeconds(0.15f);
            alarm.Trigger();
            
            yield return new WaitForSeconds(alarm.Sound.clip.length);
            _isTriggered = false;
            _animator.SetBool(Triggered, false);
        }
    }
}