/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.InteractionSystem.Interfaces;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SoundSystem.ScriptableObjects;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;

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
        [SerializeField] private NavMeshSurface navMeshSurface;
        
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public bool OnInteract(GameObject player)
        {
            if (GameManager.Instance.InventorySystem.PlayerInventory.ContainsKeyItem(key) || !requiresKey)
            {
                GameManager.Instance.SoundSystem.Play(interactSound, transform);
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

        public UnityEvent OnInteractEvents { get; }
        public UnityEvent OnInteractFailedEvents { get; }

        public void SetPassable()
        {
            transform.GetChild(0).GetComponent<NavMeshModifier>().area = 0;
            GetComponentInChildren<Collider2D>().gameObject.SetActive(false);
            // Set child game object NavigationModifier area type to Walkable
            
            // Update NavMesh
            navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
        }
    }
}
