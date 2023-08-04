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

namespace Runtime.InteractionSystem.Objects
{
    public class HospitalBed : MonoBehaviour, IInteractable
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
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

        public UnityEvent OnInteractEvents { get; }
    }
}
