/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections.Generic;
using System.Linq;
using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects
{
    public class PowerBreaker : MonoBehaviour, IInteractable
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private List<GameObject> connectedObjects = new();
        [SerializeField] private bool startPowered;
        
        //----- Interface Properties -----//
        private bool _isPowered;

        //----- Private Variables -----//
        private Animator _animator;
        private static readonly int Powered = Animator.StringToHash("powered");

        //========================= Unity events =========================//
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _isPowered = startPowered;
        }

        private void Start()
        {
            SetPowered(startPowered);
        }

        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.SoundSystem.Play(interactSound, transform.GetComponent<AudioSource>());
            _isPowered = !_isPowered;
            SetPowered(_isPowered);
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            throw new System.NotImplementedException();
        }

        public bool CanInteract()
        {
            return true;
        }

        //========================= Public methods =========================//
        
        public void SetPowered(bool state)
        {
            foreach (var powerObject in connectedObjects.Select(poweredObject => poweredObject.GetComponent<IPowered>()).Where(poweredState => poweredState != null))
            {
                if(state) powerObject.PowerOn();
                else powerObject.PowerOff();
            }
            
            _animator.SetBool(Powered, state);
        }

    }
}
