/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.InteractionSystem.Objects
{
    public class Computer : MonoBehaviour, IInteractable, IPowered
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private Sound poweredOffSound;
        
        [SerializeField] private LinkedDoor linkedDoor;
        private Animator _animator;
        private static readonly int Powered = Animator.StringToHash("IsPowered");

        private bool _used;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public bool OnInteract(GameObject player)
        {
            if (CanInteract())
            {
                GameManager.Instance.SoundSystem.Play(interactSound, transform);
                linkedDoor.Open();
                _used = true;
                return true;
            }
            
            GameManager.Instance.SoundSystem.Play(poweredOffSound, transform);
            return false;
        }

        public bool CanInteract()
        {
            return IsPowered && !_used;
        }

        public UnityEvent OnInteractEvents { get; }
        public UnityEvent OnInteractFailedEvents { get; }
        public bool failedToInteract { get; set; }

        public void PowerOn()
        {
            _animator.SetBool(Powered, true);
            IsPowered = true;
        }
        
        public void PowerOff()
        {
            _animator.SetBool(Powered, false);
            IsPowered = false;
        }

        public bool IsPowered { get; set; }
    }
}
