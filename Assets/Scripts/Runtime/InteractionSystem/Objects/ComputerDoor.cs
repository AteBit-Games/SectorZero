/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.Managers;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects
{
    /// <summary>
    /// Linked to a computer, this door will open when the computer is powered on and interacted with.
    /// </summary>
    public class LinkedDoor : MonoBehaviour
    {
        [SerializeField] private Sound openSound;
        public Sound InteractSound => openSound;
        
        private bool _isActivated;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Open()
        {
            if(_isActivated) return;
            
            GameManager.Instance.SoundSystem.Play(openSound, transform);
            _animator.SetTrigger("OpenDoor");
            _isActivated = true;
        }
    }
}