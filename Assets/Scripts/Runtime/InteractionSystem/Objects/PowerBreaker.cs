/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System.Collections.Generic;
using System.Linq;
using Runtime.AI;
using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects
{
    public class PowerBreaker : MonoBehaviour, IInteractable, IPowered
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private List<GameObject> connectedObjects = new();
        [SerializeField] private bool startPowered;
        
        //----- Interface Properties -----//
        public bool IsPowered { get; set; }

        //----- Private Variables -----//
        private Animator _animator;
        private static readonly int IsEnabled = Animator.StringToHash("IsEnabled");
        private NoiseEmitter _noiseEmitter;

        //========================= Unity events =========================//
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            IsPowered = startPowered;
            _noiseEmitter = GetComponentInChildren<NoiseEmitter>();
        }

        private void Start()
        {
            SetPowered(startPowered);
        }

        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.SoundSystem.Play(interactSound, transform.GetComponent<AudioSource>());
            IsPowered = !IsPowered;
            SetPowered(IsPowered);
            _noiseEmitter.EmitGlobal();
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

        public void PowerOn()
        {
            IsPowered = true;
        }

        public void PowerOff()
        {
            IsPowered = false;
        }
        
        //========================= Public methods =========================//
        
        public void SetPowered(bool state)
        {
            foreach (var powerObject in connectedObjects.Select(poweredObject => poweredObject.GetComponent<IPowered>()).Where(poweredState => poweredState != null))
            {
                if(state)
                {
                    powerObject.PowerOn();
                    PowerOn();
                }
                else
                {
                    powerObject.PowerOff();
                    PowerOff();
                }
            }
            _animator.SetBool(IsEnabled, state);
        }

    }
}
