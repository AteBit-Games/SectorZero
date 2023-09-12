/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections.Generic;
using System.Linq;
using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Runtime.InteractionSystem.Objects.Powered
{
    [DefaultExecutionOrder(22)]
    public class LockdownSwitch : MonoBehaviour, IInteractable, IPersistant
    {
        [SerializeField] public string persistentID;
        [SerializeField] private Sound humSound;
        [SerializeField] private Sound failSound;
        [SerializeField] private Sound onSound;
        public Sound InteractSound => onSound;

        [SerializeField] private SecurityTerminal securityTerminal;
        [SerializeField] private List<FuseBox> fuseBoxes;
        [SerializeField] private Animator meterAnimator;
        
        [SerializeField] private bool debug;

        //----- Private Variables -----//
        private readonly Dictionary<FuseBox, bool> _fuseBoxStates = new();
        private readonly List<Light2D> _stareLights = new();
        private bool _canBePowered;
        private bool _isPowered;
        
        private Animator _animator;
        private Light2D _mainLight;
        private AudioSource _audioSource;
        
        
        private static readonly int Failure = Animator.StringToHash("fail");
        private static readonly int Powered = Animator.StringToHash("powered");
        private static readonly int ActiveFuseBoxes = Animator.StringToHash("activeFuseBoxes");

        //========================= Unity events =========================//
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _mainLight = GetComponentInChildren<Light2D>();
            _audioSource = GetComponent<AudioSource>();
            GameManager.Instance.SoundSystem.SetupSound(_audioSource, onSound);
            
            _stareLights.AddRange(meterAnimator.gameObject.GetComponentsInChildren<Light2D>());
            foreach (var fuseBox in fuseBoxes)
            {
                fuseBox.PowerStateChanged += OnPowerStateChanged;
                _fuseBoxStates.Add(fuseBox, fuseBox.IsPowered);
            }
            
            SetActiveState(_fuseBoxStates.Count(isPowered => !isPowered.Value));
            _canBePowered = _fuseBoxStates.Values.All(powered => !powered);
            _mainLight.enabled = _canBePowered;
            
            if(debug)
            {
                securityTerminal.PowerOn(false);
            }
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
                
                GameManager.Instance.SoundSystem.PlayOneShot(failSound, _audioSource);
                securityTerminal.PowerOff();
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
                
                GameManager.Instance.SoundSystem.PlayOneShot(onSound, _audioSource);
                GameManager.Instance.SoundSystem.Play(humSound, _audioSource);
                securityTerminal.PowerOn(false);
            }
            else
            {
                GameManager.Instance.SoundSystem.PlayOneShot(failSound, _audioSource);
                _animator.SetTrigger(Failure);
            }
            
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
        
        //=========================== Save System =============================//

        public string LoadData(SaveGame game)
        {
            if(game.worldData.mainFuseStatus)
            {
                _animator.SetBool(Powered, true);
                _mainLight.enabled = true;
                _isPowered = true;
                GameManager.Instance.SoundSystem.Play(humSound, _audioSource);
                securityTerminal.PowerOn(true);
            }
            
            return persistentID;
        }

        public void SaveData(SaveGame game)
        {
            
            game.worldData.mainFuseStatus = _isPowered;
        }
    }
}
