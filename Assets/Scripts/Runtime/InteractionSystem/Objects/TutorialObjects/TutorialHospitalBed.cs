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
using UnityEngine.Rendering;

namespace Runtime.InteractionSystem.Objects.TutorialObjects
{
    public class TutorialHospitalBed : MonoBehaviour, IInteractable
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        [SerializeField] private Animator towelAnimator;
        
        //----- Private Variables -----//
        private Collider2D _interactionCollider;
        private bool active;
        private static readonly int Start = Animator.StringToHash("Start");

        //========================= Unity Events =========================//
        
        private void Awake()
        {
            TutorialManager.StartListening("TutorialStage2", Init);
            _interactionCollider = GetComponent<Collider2D>();
            
            // Disable interaction
            gameObject.layer = 0;
            _interactionCollider.enabled = false;
        }
        
        //========================= Interface Methods =========================//

        public bool OnInteract(GameObject player)
        {
            var nellient = player.GetComponentInParent<TutorialNellient>();
            GameManager.Instance.SoundSystem.Play(interactSound, transform.GetComponent<AudioSource>());
            nellient.StartGame();
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
        
        //========================= Public Methods =========================//
        
        public void DropTowel()
        {
            towelAnimator.SetTrigger(Start);
        }
        
        public void TowelEnd()
        {
            var towel = towelAnimator.gameObject.GetComponent<SortingGroup>();
            towel.sortingOrder = 9;
            towel.sortAtRoot = true;
        }
        
        //========================= Private Methods =========================//
        
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
    }
}
