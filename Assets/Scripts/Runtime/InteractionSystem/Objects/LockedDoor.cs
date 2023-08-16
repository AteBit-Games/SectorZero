/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.InteractionSystem.Interfaces;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SoundSystem;
using Unity.AI.Navigation;
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
        [SerializeField] private NavMeshSurface navMeshSurface;
        
        private Animator _animator;
        private static readonly int OpenDoor = Animator.StringToHash("OpenDoor");

        //========================= Unity Events =========================//

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.SoundSystem.Play(interactSound, transform.GetComponent<AudioSource>());
            _animator.SetTrigger(OpenDoor);
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            GameManager.Instance.SoundSystem.Play(lockedSound, transform.GetComponent<AudioSource>());
        }

        public bool CanInteract()
        {
            return GameManager.Instance.InventorySystem.PlayerInventory.ContainsKeyItem(key) || !requiresKey;
        }

        //========================= Public methods =========================//

        public void SetPassable()
        {
            transform.GetChild(0).GetComponent<NavMeshModifier>().area = 0;
            GetComponentInChildren<Collider2D>().gameObject.SetActive(false);
            navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
        }
    }
}
