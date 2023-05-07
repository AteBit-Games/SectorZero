/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects
{
    /// <summary>
    /// Standalone locked door. Can be opened with a key or without when the door is unlocked.
    /// </summary>
    public class LockedDoor : MonoBehaviour, IInteractable
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private Sound lockedSound;

        [SerializeField] private bool requiresKey;
        [SerializeField] private Item key;
        
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.SoundSystem.Play(interactSound, transform);

            if (GameManager.Instance.InventorySystem.PlayerInventory.ContainsKeyItem(key) || !requiresKey)
            {
                _animator.SetTrigger("OpenDoor");
                return true;
            }
            else
            {
                GameManager.Instance.SoundSystem.Play(lockedSound, transform);
                return false;
            }
        }
        
        public bool CanInteract()
        {
            return GameManager.Instance.InventorySystem.PlayerInventory.ContainsKeyItem(key) || !requiresKey;
        }
    }
}
