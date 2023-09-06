/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.InteractionSystem.Interfaces;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Runtime.InteractionSystem.Objects.Powered
{
    [DefaultExecutionOrder(20)]
    public class FuseBox : MonoBehaviour, IInteractable, IPersistant
    {
        [SerializeField] public string persistentID;
        [SerializeField] private Item fuse;
        [SerializeField] public bool startWithFuse;
        
        [SerializeField] private Sound humSound;
        [SerializeField] private Sound noFuseSound;
        [SerializeField] private Sound addFuseSound;
        
        [SerializeField] private Sound offSound;
        [SerializeField] private Sound onSound;
        public Sound InteractSound => onSound;
        
        [SerializeField] private List<GameObject> connectedObjects = new();
        [SerializeField] private bool startPowered;
        
        public event Action<FuseBox, bool> PowerStateChanged;
        
        //----- Interface Properties -----//
        private bool _hasFuse;
        private bool _isPowered;
        public bool IsPowered => _isPowered;

        //----- Private Variables -----//
        private Animator _animator;
        private Light2D _light;
        private AudioSource _audioSource;
        
        private static readonly int NoFuse = Animator.StringToHash("noFuse");
        private static readonly int AddFuse = Animator.StringToHash("addFuse");
        private static readonly int Powered = Animator.StringToHash("powered");

        //========================= Unity events =========================//
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _light = transform.parent.gameObject.GetComponentInChildren<Light2D>();
            
            _audioSource = GetComponent<AudioSource>();
            _audioSource.outputAudioMixerGroup = humSound.mixerGroup;
        }

        private void Start()
        {
            if (GameManager.Instance.TestMode)
            {
                _hasFuse = startWithFuse;
                if (_hasFuse)
                {
                    SetPowered(startPowered);
                    _isPowered = startPowered;
                }
                else
                {
                    SetPowered(false);
                    _animator.SetTrigger(NoFuse);
                }
            }
        }

        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            if(!_hasFuse)
            {
                _audioSource.PlayOneShot(addFuseSound.clip);
                _animator.SetTrigger(AddFuse);
                GameManager.Instance.InventorySystem.PlayerInventory.UseItemInInventory(fuse);
                GameManager.Instance.SaveSystem.SaveGame();
                _hasFuse = true;
            }
            else
            {
                _isPowered = !_isPowered;
                SetPowered(_isPowered);
                _audioSource.PlayOneShot(_isPowered ? onSound.clip : offSound.clip);
            }
            
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            _audioSource.PlayOneShot(noFuseSound.clip);
        }

        public bool CanInteract()
        {
            return _hasFuse || GameManager.Instance.InventorySystem.PlayerInventory.ContainsKeyItem(fuse);
        }

        //========================= Public methods =========================//

        private void SetPowered(bool state)
        {
            foreach (var powerObject in connectedObjects.Select(item => item.GetComponent<IPowered>() ?? item.GetComponentInChildren<IPowered>()))
            {
                if(state) powerObject.PowerOn();
                else powerObject.PowerOff();
            }
            
            PowerStateChanged?.Invoke(this, state);
            _animator.SetBool(Powered, state);
            _light.enabled = state;
            
            if(state) GameManager.Instance.SoundSystem.Play(humSound, _audioSource);
            else _audioSource.Stop();
        }

        //=========================== Save System =============================//
        
        public string LoadData(SaveGame game)
        {
            if (game.worldData.fuseBoxes.TryGetValue(persistentID, out var fusebox))
            {
                if (fusebox == 2)
                {
                    _hasFuse = false;
                    SetPowered(false);
                    _isPowered = false;
                    _animator.SetTrigger(NoFuse);
                }
                else
                {
                    _hasFuse = true;
                    _isPowered = fusebox == 1;
                    SetPowered(fusebox == 1);
                }
            }
            else
            {
                _hasFuse = startWithFuse;
                if (_hasFuse)
                {
                    _isPowered = startPowered;
                    SetPowered(startPowered);
                }
                else
                {
                    SetPowered(false);
                    _isPowered = false;
                    _animator.SetTrigger(NoFuse);
                }
            }
            
            return persistentID;
        }

        public void SaveData(SaveGame game)
        {
            if (_hasFuse) game.worldData.fuseBoxes[persistentID] = _isPowered ? 1 : 0;
            else game.worldData.fuseBoxes[persistentID] = 2;
        }
    }
}
