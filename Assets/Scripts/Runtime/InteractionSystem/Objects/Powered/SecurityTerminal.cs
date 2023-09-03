/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using Runtime.InteractionSystem.Interfaces;
using Runtime.InteractionSystem.Objects.Doors;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects.Powered
{
    public class SecurityTerminal : MonoBehaviour, IInteractable, IPowered
    {
        [SerializeField] private Sound lockedSound;
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private Item keyCard;
        [SerializeField] private TriggerDoor door;

        //----- Interface Properties -----//
        private bool _isPowered;

        public bool IsPowered { get; set; }

        //----- Private Variables -----//
        private Animator _animator;
        private AudioSource _audioSource;
        
        //----- Animator Hashes -----//
        private static readonly int Powered = Animator.StringToHash("powered");
        private static readonly int Unlock = Animator.StringToHash("unlock");
        private static readonly int Locked = Animator.StringToHash("locked");

        //========================= Unity events =========================//

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
        }

        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.SoundSystem.Play(interactSound, _audioSource);
            _animator.SetTrigger(Unlock);
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            switch (_isPowered)
            {
                case false:
                    GameManager.Instance.SoundSystem.Play(lockedSound, _audioSource);
                    break;
                case true when !GameManager.Instance.InventorySystem.PlayerInventory.ContainsKeyItem(keyCard):
                    GameManager.Instance.SoundSystem.Play(lockedSound, _audioSource);
                    _animator.SetTrigger(Locked);
                    break;
            }
        }

        public bool CanInteract()
        {
            return _isPowered && GameManager.Instance.InventorySystem.PlayerInventory.ContainsKeyItem(keyCard);
        }

        public void TriggerDoor()
        {
            door.OpenDoor();
        }

        //========================= Public methods =========================//
        
        public void PowerOn()
        {
            _isPowered = true;
            _animator.SetBool(Powered, true);
        }

        public void PowerOff()
        {
            _isPowered = false;
            _animator.SetBool(Powered, false);
        }
    }
}