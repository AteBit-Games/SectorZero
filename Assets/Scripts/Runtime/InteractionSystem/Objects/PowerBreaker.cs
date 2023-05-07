/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System.Collections.Generic;
using System.Linq;
using Runtime.Managers;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.InteractionSystem.Objects
{
    public class PowerBreaker : MonoBehaviour, IInteractable, IPowered
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private List<GameObject> connectedObjects = new();
        [SerializeField] private bool startPowered;
        
        private bool _isActivated;
        private Animator _animator;
        private static readonly int IsEnabled = Animator.StringToHash("IsEnabled");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            IsPowered = startPowered;
            SetPowered(IsPowered);
        }

        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.SoundSystem.Play(interactSound, transform);
            _isActivated = !_isActivated;
            SetPowered(_isActivated);
            return true;
        }

        public void SetPowered(bool powered)
        {
            foreach (var powerObject in connectedObjects.Select(poweredObject => poweredObject.GetComponent<IPowered>()).Where(poweredState => poweredState != null))
            {
                if (powered)
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
            _animator.SetBool(IsEnabled, powered);
        }

        public bool CanInteract()
        {
            return true;
        }

        public bool IsPowered { get; set; }
        public void PowerOn()
        {
            IsPowered = true;
        }

        public void PowerOff()
        {
            IsPowered = true;
        }
    }
}
