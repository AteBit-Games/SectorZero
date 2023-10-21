/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using Runtime.InteractionSystem.Interfaces;
using Runtime.InventorySystem;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.Misc.Objects;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.InteractionSystem.Objects.UI
{
    public class Safe : MonoBehaviour, IInteractable, IPersistant
    {
        [SerializeField] public Item safeItem;
        [SerializeField] public Sound itemPickupSound;
        [SerializeField] private Alarm safeAlarm; 
        
        [SerializeField] public SummaryEntry summaryEntry;
        [SerializeField] public string safeCode;
        [SerializeField] private Sound interactSound;
        
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
            
            var inventory = player.GetComponentInParent<PlayerInventory>();
            if(!inventory.ContainsSummaryEntry(summaryEntry)) inventory.AddSummaryEntry(summaryEntry);
            
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
            AddItem();
            
            _player.GetComponentInParent<PlayerInventory>().SetSummaryEntryCompleted(summaryEntry);
            
            //Open the safe
            _animator.SetTrigger(Open);
            _isOpen = true;
            
            GameManager.Instance.SaveSystem.SaveGame();
        }

        public void TriggerAlarm()
        {
            safeAlarm.Trigger();
        }

        private void DisableInteraction()
        {
            gameObject.layer = 0;
            GetComponent<Collider2D>().enabled = false;
        }

        private void AddItem()
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
