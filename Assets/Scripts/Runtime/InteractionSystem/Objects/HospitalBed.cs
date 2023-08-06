/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.Player;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Runtime.InteractionSystem.Objects
{
    public class HospitalBed : MonoBehaviour, IInteractable
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;

        [SerializeField] private Animator _towelAnimator;
        
        private Collider2D _interactionCollider;

        private bool active;

        private void Awake()
        {
            TutorialManager.StartListening("TutorialStage2", Init);
            _interactionCollider = GetComponent<Collider2D>();
            
            // Disable interaction
            gameObject.layer = 0;
            _interactionCollider.enabled = false;
        }

        public bool OnInteract(GameObject player)
        {
            var nellient = player.GetComponentInParent<Nellient>();
            GameManager.Instance.SoundSystem.Play(interactSound, transform);
            nellient.StartGame();
            Finish();
            return true;
        }

        private void Init()
        {
            active = true;
            gameObject.layer = 3;
            _interactionCollider.enabled = true;
        }
        
        private void Finish()
        {
            active = false;
            gameObject.layer = 0;
        }
        
        public bool CanInteract()
        {
            return active;
        }
        
        public void DropTowel()
        {
            _towelAnimator.SetTrigger("Start");
        }
        
        public void TowelEnd()
        {
            var towel = _towelAnimator.gameObject.GetComponent<SortingGroup>();
            towel.sortingOrder = 9;
            towel.sortAtRoot = true;
        }

        public UnityEvent OnInteractEvents { get; }
        public UnityEvent OnInteractFailedEvents { get; }
        public bool failedToInteract { get; set; }
    }
}
