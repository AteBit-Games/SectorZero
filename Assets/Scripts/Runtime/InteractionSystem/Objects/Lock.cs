/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using Runtime.InteractionSystem.Interfaces;
using Runtime.InteractionSystem.Objects.Doors;
using Runtime.InventorySystem;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class Lock : MonoBehaviour, IInteractable, IPersistant
    {
        [SerializeField] public TriggerDoor door;
        [SerializeField] private Item key;
        [SerializeField] public string persistentID;

        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private Sound lockedSound;

        private readonly int _locked = Animator.StringToHash("locked");
        private Animator _mainAnimator;
        private bool _canInteract = true;
        
        //=========================== Unity Events =============================//
        
        private void Start()
        {
            _mainAnimator = GetComponent<Animator>();
        }

        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.SoundSystem.Play(interactSound, transform.GetComponent<AudioSource>());
            player.GetComponentInParent<PlayerInventory>().UseItemInInventory(key);
            gameObject.SetActive(false);
            door.OpenDoor();
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            if(!_canInteract) return;
            
            GameManager.Instance.SoundSystem.Play(lockedSound, transform.GetComponent<AudioSource>());
            _mainAnimator.SetTrigger(_locked);
            _canInteract = false;
        }

        public bool CanInteract()
        {
            return GameManager.Instance.InventorySystem.PlayerInventory.ContainsKeyItem(key);
        }
        
        //========================= Helper Methods =========================//
        
        private void EnableInteraction()
        {
            _canInteract = true;
        }

        //=========================== Save System =============================//
        
        public void LoadData(SaveGame game)
        {
            if (game.worldData.miscItems.TryGetValue(persistentID, out var active)) gameObject.SetActive(active);
        }

        public void SaveData(SaveGame game)
        {
            game.worldData.miscItems[persistentID] = gameObject.activeSelf;
        }
    }
}
