/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using Runtime.InteractionSystem.Interfaces;
using Runtime.InventorySystem;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects.Doors
{
    [DefaultExecutionOrder(6)]
    [RequireComponent(typeof(Animator))]
    public class PryDoor : MonoBehaviour, IInteractable, IPersistant
    {
        [SerializeField] private Item requiredItem;
        [SerializeField] public SummaryEntry summaryEntry;
        [SerializeField] private Item itemToAdd;
        [SerializeField] public string persistentID;

        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private Sound failSound;

        private readonly int _failed = Animator.StringToHash("stuck");
        private readonly int _open = Animator.StringToHash("open");
        
        private Animator _mainAnimator;
        private bool _isOpened;
        
        //=========================== Unity Events =============================//

        private void Awake()
        {
            _mainAnimator = GetComponent<Animator>();
        }

        private void Start()
        {
            if (requiredItem == null) Debug.LogWarning("No required item set for " + gameObject.name);
            if (summaryEntry == null) Debug.LogWarning("No summary entry set for " + gameObject.name);
            if (string.IsNullOrEmpty(persistentID)) Debug.LogWarning("No persistent ID set for pry door");
        }

        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.SoundSystem.Play(interactSound);
            _mainAnimator.SetTrigger(_open);
            
            var inventory = player.GetComponentInParent<PlayerInventory>();
            GameManager.Instance.NotificationManager.ShowPickupNotification(itemToAdd);
            
            player.GetComponent<PlayerInteraction>().RemoveInteractable(gameObject);
            DisableInteraction();
            
            _isOpened = true;
            return inventory.AddItemToInventory(itemToAdd);
        }

        public void OnInteractFailed(GameObject player)
        {
            if (!player.GetComponentInParent<PlayerInventory>().ContainsSummaryEntry(summaryEntry))  
                GameManager.Instance.InventorySystem.PlayerInventory.AddSummaryEntry(summaryEntry);
            
            GameManager.Instance.SoundSystem.Play(failSound);
            _mainAnimator.SetTrigger(_failed);
        }

        public bool CanInteract()
        {
            return GameManager.Instance.InventorySystem.PlayerInventory.ContainsKeyItem(requiredItem);
        }
        
        protected void DisableInteraction()
        {
            gameObject.layer = 0;
            GetComponent<Collider2D>().enabled = false;
        }

        //=========================== Save System =============================//
        
        public string LoadData(SaveGame game)
        {
            if(game.worldData.miscItems.TryGetValue(persistentID, out var opened))
            {
                _isOpened = opened;
                if(_isOpened)
                {
                    _mainAnimator.SetTrigger(_open);
                    DisableInteraction();
                }
            }
            
            return persistentID;
        }

        public void SaveData(SaveGame game)
        {
            game.worldData.miscItems[persistentID] = _isOpened;
        }
    }
}