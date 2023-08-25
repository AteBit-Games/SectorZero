/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.Player.Nellient;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects
{
    public class HospitalBed : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject playerObject;
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        private Collider2D _interactionCollider;
        private bool active;
        
        //========================== Unity Events ========================//

        private void Awake()
        {
            _interactionCollider = GetComponent<Collider2D>();
            _interactionCollider.enabled = false;
            gameObject.layer = 0;
        }

        //========================== Interface Events ========================//
        
        public bool OnInteract(GameObject player)
        {
            playerObject.SetActive(true);
            var nellient = player.GetComponentInParent<Nellient>();
            nellient.gameObject.SetActive(false);
            
            GameManager.Instance.SoundSystem.Play(interactSound, transform.GetComponent<AudioSource>());
            Finish();
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            throw new NotImplementedException();
        }

        public bool CanInteract()
        {
            return active;
        }
        
        //========================== Private Methods ========================//
        
        public void Init()
        {
            active = true;
            gameObject.layer = 3;
            _interactionCollider.enabled = true;
        }

        private void Finish()
        {
            active = false;
            gameObject.layer = 0;
            _interactionCollider.enabled = false;
        }
    }
}
