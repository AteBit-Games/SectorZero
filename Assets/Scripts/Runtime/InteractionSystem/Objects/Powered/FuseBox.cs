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
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Runtime.InteractionSystem.Objects.Powered
{
    [DefaultExecutionOrder(20)]
    public class FuseBox : MonoBehaviour, IInteractable
    {
        [SerializeField] private Item fuse;
        [SerializeField] private bool startWithFuse;
        
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
            
            _isPowered = startPowered;
        }

        private void Start()
        {
            _hasFuse = startWithFuse;
            if (_hasFuse)
            {
                SetPowered(startPowered);
            }
            else
            {
                SetPowered(false);
                _animator.SetTrigger(NoFuse);
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

    }
}
