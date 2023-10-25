/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Runtime.InteractionSystem.Interfaces;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Runtime.InteractionSystem.Objects.Powered
{
    [DefaultExecutionOrder(6)]
    public class LockdownSwitch : MonoBehaviour, IInteractable, IPersistant
    {
        [SerializeField] public string persistentID;
        [SerializeField] private Sound humSound;
        [SerializeField] private Sound failSound;
        [SerializeField] private Sound onSound;
        public Sound InteractSound => onSound;

        [SerializeField] private ElevatorPanel elevatorPanel;
        [SerializeField] private List<FuseBox> fuseBoxes;
        [SerializeField] private Animator meterAnimator;
        
        [SerializeField] private bool debug;
        [SerializeField] public SummaryEntry summaryEntry;

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
        }

        private void Start()
        {
            if(debug)
            {
                elevatorPanel.PowerOn(false);
                _isPowered = true;
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
                elevatorPanel.PowerOff();
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

        private void FlickerLights()
        {
            var activeLights = _stareLights.Where(light => light.enabled).ToList();
            foreach (var light in activeLights)
            {
                StartCoroutine(FlickerLight(light));
            }
        }
        
        private static IEnumerator FlickerLight([NotNull] Light2D light)
        {
            if (light == null) throw new ArgumentNullException(nameof(light));
            var flickerCount = 0;
            while (flickerCount < 3)
            {
                light.enabled = false;
                yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
                light.enabled = true;
                yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
                flickerCount++;
            }
        }

        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            _animator.SetBool(Powered, true);
            _mainLight.enabled = true;
            _isPowered = true;
                
            GameManager.Instance.SoundSystem.PlayOneShot(onSound, _audioSource);
            GameManager.Instance.SoundSystem.Play(humSound, _audioSource);
            elevatorPanel.PowerOn(false);
            
            player.GetComponent<PlayerInteraction>().RemoveInteractable(gameObject);
            GameManager.Instance.InventorySystem.PlayerInventory.SetSummaryEntryCompleted(summaryEntry);
            
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            if (!_isPowered)
            {
                FlickerLights();
                GameManager.Instance.SoundSystem.PlayOneShot(failSound, _audioSource);
                _animator.SetTrigger(Failure);
            }
        }

        public bool CanInteract()
        {
            return _canBePowered && !_isPowered;
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
                elevatorPanel.PowerOn(true);
            }
            
            return persistentID;
        }

        public void SaveData(SaveGame game)
        {
            game.worldData.mainFuseStatus = _isPowered;
        }
    }
}
