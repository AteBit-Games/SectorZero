/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections.Generic;
using System.Linq;
using Runtime.InteractionSystem.Interfaces;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Runtime.InteractionSystem.Objects.Powered
{
    [DefaultExecutionOrder(22)]
    public class LockdownSwitch : MonoBehaviour, IInteractable
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;

        [SerializeField] private List<FuseBox> fuseBoxes;
        [SerializeField] private Animator meterAnimator;

        //----- Private Variables -----//
        private readonly Dictionary<FuseBox, bool> _fuseBoxStates = new();
        private readonly List<Light2D> _stareLights = new();
        private bool _canBePowered;
        private bool _isPowered;
        
        private Animator _animator;
        private Light2D _mainLight;
        private static readonly int Failure = Animator.StringToHash("fail");
        private static readonly int Powered = Animator.StringToHash("powered");
        private static readonly int ActiveFuseBoxes = Animator.StringToHash("activeFuseBoxes");

        //========================= Unity events =========================//
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _mainLight = GetComponentInChildren<Light2D>();
            _stareLights.AddRange(meterAnimator.gameObject.GetComponentsInChildren<Light2D>());

            foreach (var fuseBox in fuseBoxes)
            {
                fuseBox.PowerStateChanged += OnPowerStateChanged;
                _fuseBoxStates.Add(fuseBox, fuseBox.IsPowered);
            }
            
            SetActiveState(_fuseBoxStates.Count(isPowered => !isPowered.Value));
            _canBePowered = _fuseBoxStates.Values.All(powered => !powered);
            _mainLight.enabled = _canBePowered;
        }

        private void OnPowerStateChanged(FuseBox fuseBox, bool state)
        {
            _fuseBoxStates[fuseBox] = state;
            _canBePowered = _fuseBoxStates.Values.All(powered => !powered);
            SetActiveState(_fuseBoxStates.Count(isPowered => !isPowered.Value));

            if(_isPowered && !_canBePowered)
            {
                _animator.SetBool(Powered, false);
                _isPowered = false;
                _mainLight.enabled = false;
            }
        }
        
        private void SetActiveState(int activeFuseBoxes)
        {
            meterAnimator.SetInteger(ActiveFuseBoxes, activeFuseBoxes);
            
            for(var i = 0; i < _stareLights.Count; i++)
            {
                _stareLights[i].enabled = i < activeFuseBoxes;
            }
        }

        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            if(_canBePowered)
            {
                _animator.SetBool(Powered, true);
                _mainLight.enabled = true;
                _isPowered = true;
            }
            else
            {
                _animator.SetTrigger(Failure);
            }
            
            // GameManager.Instance.SoundSystem.Play(interactSound, transform.GetComponent<AudioSource>());
            // _isPowered = !_isPowered;
            // SetPowered(_isPowered);
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
    }
}
