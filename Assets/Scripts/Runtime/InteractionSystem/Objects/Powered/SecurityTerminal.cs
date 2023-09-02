/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.InteractionSystem.Interfaces;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects.Powered
{
    public class SecurityTerminal : MonoBehaviour, IInteractable, IPowered
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private Animator readerAnimator;
        [SerializeField] private Animator terminalAnimator;
        
        //----- Interface Properties -----//
        private bool _isPowered;

        public bool IsPowered { get; set; }

        public void PowerOn()
        {
            _isPowered = true;
        }

        public void PowerOff()
        {
            _isPowered = false;
        }

        //----- Private Variables -----//
        private static readonly int Powered = Animator.StringToHash("powered");

        //========================= Unity events =========================//
        

        private void Start()
        {
            
        }

        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {

            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            
        }

        public bool CanInteract()
        {
            return _isPowered;
        }

        //========================= Public methods =========================//

    }
}
