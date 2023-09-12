/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using Runtime.InteractionSystem.Interfaces;
using Runtime.InventorySystem;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects.UI
{
    public class Safe : MonoBehaviour, IInteractable, IPersistant
    {
        [SerializeField] public Item safeItem;
        [SerializeField] public Sound itemPickupSound;
        
        [SerializeField] public string safeCode;
        [SerializeField] private Sound interactSound;
        [SerializeField] private Sound safeOpenSound;
        
        public Sound InteractSound => interactSound;
        
        private GameObject _player;
        private Animator _animator;
        private bool _isOpen;
        private static readonly int Open = Animator.StringToHash("open");

        //=============================== Unity Events ===============================//
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        
        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.SoundSystem.Play(interactSound);
            GameManager.Instance.HUD.OpenKeyPad(this);
            _player = player;
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            throw new NotImplementedException();
        }
        
        public bool CanInteract()
        {
            return !_isOpen;
        }
        
        //========================= Safe =========================//

        public void OpenSafe()
        {
            //Disable the collider and interaction
            _player.GetComponent<PlayerInteraction>().RemoveInteractable(gameObject);
            DisableInteraction();
            
            //Open the safe
            GameManager.Instance.SoundSystem.Play(safeOpenSound);
            _animator.SetTrigger(Open);
            _isOpen = true;
        }

        private void DisableInteraction()
        {
            gameObject.layer = 0;
            GetComponent<Collider2D>().enabled = false;
        }

        public void AddItem()
        {
            GameManager.Instance.SoundSystem.Play(itemPickupSound);
            var inventory = _player.GetComponentInParent<PlayerInventory>();
            
            GameManager.Instance.NotificationManager.ShowPickupNotification(safeItem);
            inventory.AddItemToInventory(safeItem);
        }
        
        //========================= Save System =========================//

        public string LoadData(SaveGame game)
        {
            if (game.worldData.safeOpen)
            {
                DisableInteraction();
                _animator.SetTrigger(Open);
            }
            return "Safe";
        }

        public void SaveData(SaveGame game)
        {
            game.worldData.safeOpen = _isOpen;
        }
    }
}
