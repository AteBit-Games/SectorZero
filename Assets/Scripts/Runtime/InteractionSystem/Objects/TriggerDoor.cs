/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using Runtime.InteractionSystem.Interfaces;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SoundSystem.ScriptableObjects;
using NavMeshPlus.Components;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects
{
    /// <summary>
    /// Standalone locked door. Can be opened with a key or without when the door is unlocked.
    /// </summary>
    public class TriggerDoor : MonoBehaviour
    {
        [SerializeField] private Sound openSound;
        [SerializeField] private Sound closeSound;
        [SerializeField] private NavMeshSurface navMeshSurface;
        [SerializeField] private Collider2D navigationBlocker;
        [SerializeField] private Animator bottomAnimator;
        [SerializeField] private bool startOpen;

        private Animator _mainAnimator;
        
        // ============ Animator Hashes ============
        private readonly int _isOpen = Animator.StringToHash("isOpen");

        private void Awake()
        {
            _mainAnimator = GetComponent<Animator>();
            if (startOpen)
            {
                _mainAnimator.SetBool(_isOpen, true);
            }
        }
        
        public void OpenDoor()
        {
            GameManager.Instance.SoundSystem.Play(openSound, transform);
            _mainAnimator.SetBool(_isOpen, true);
        }

        public void CloseDoor()
        {
            GameManager.Instance.SoundSystem.Play(closeSound, transform);
            _mainAnimator.SetBool(_isOpen, false);
        }
        
        public void TriggerHit()
        {
            _mainAnimator.SetTrigger("hit");
        }

        public void BreakDoor()
        {
            _mainAnimator.SetTrigger("break");
        }

        public void TriggerBottom()
        {
            bottomAnimator.SetTrigger("break");
        }

        public void SetBlocker(int enabled)
        {
            navigationBlocker.gameObject.SetActive(enabled > 0);
        }

        public void UpdateMesh()
        {
            navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
        }

    }
}
