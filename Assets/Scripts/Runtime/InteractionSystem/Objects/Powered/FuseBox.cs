/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Runtime.InteractionSystem.Objects.Powered
{
    [DefaultExecutionOrder(20)]
    public class FuseBox : MonoBehaviour, IInteractable
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private List<GameObject> connectedObjects = new();
        [SerializeField] private bool startPowered;
        
        public event Action<FuseBox, bool> PowerStateChanged;
        
        //----- Interface Properties -----//
        private bool _isPowered;
        public bool IsPowered => _isPowered;

        //----- Private Variables -----//
        private Animator _animator;
        private Light2D _light;
        private static readonly int Powered = Animator.StringToHash("powered");

        //========================= Unity events =========================//
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _light = transform.parent.gameObject.GetComponentInChildren<Light2D>();
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
            
            PowerStateChanged?.Invoke(this, state);
            _animator.SetBool(Powered, state);
            _light.enabled = state;
        }

    }
}
