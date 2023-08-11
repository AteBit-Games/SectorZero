/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects
{
    public class Computer : MonoBehaviour, IInteractable, IPowered
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private Sound poweredOffSound;
        [SerializeField] private LinkedDoor linkedDoor;

        //------- Interface Properties -------//
        public bool IsPowered { get; set; }
        
        
        //----- Private Variables -----//
        private Animator _animator;
        private static readonly int Powered = Animator.StringToHash("IsPowered");
        private bool _used;

        //========================= Unity Events =========================//
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        
        //========================= Interface events =========================//

        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.SoundSystem.Play(interactSound, transform.GetComponent<AudioSource>());
            linkedDoor.Open();
            _used = true;
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            GameManager.Instance.SoundSystem.Play(poweredOffSound, transform.GetComponent<AudioSource>());
        }

        public bool CanInteract()
        {
            return IsPowered && !_used;
        }
        
        //========================= Public Methods =========================//
        
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
    }
}
