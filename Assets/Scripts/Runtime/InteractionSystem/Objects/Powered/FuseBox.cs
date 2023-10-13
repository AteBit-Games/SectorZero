/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Runtime.InteractionSystem.Interfaces;
using Runtime.InventorySystem;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;


namespace Runtime.InteractionSystem.Objects.Powered
{
    [DefaultExecutionOrder(5)]
    public class FuseBox : MonoBehaviour, IInteractable, IPersistant, ISoundEntity
    {
        [SerializeField] public string persistentID;
        [SerializeField] private List<Item> fuse;
        [SerializeField] public bool startWithFuse;
        
        [SerializeField] public SummaryEntry summaryEntry;
        
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
        public bool IsPowered { get; private set; }

        public AudioSource AudioSource { get; private set; }
        public Sound Sound => humSound;
        public bool CanPlay { get; set; } = true;

        //----- Private Variables -----//
        private Animator _animator;
        private Light2D _light;
        
        private static readonly int NoFuse = Animator.StringToHash("noFuse");
        private static readonly int AddFuse = Animator.StringToHash("addFuse");
        private static readonly int Powered = Animator.StringToHash("powered");

        //========================= Unity events =========================//
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _light = transform.parent.gameObject.GetComponentInChildren<Light2D>();
            
            AudioSource = GetComponent<AudioSource>();
            GameManager.Instance.SoundSystem.SetupSound(AudioSource, humSound);
        }

        private void Start()
        {
            if (GameManager.Instance.TestMode)
            {
                _hasFuse = startWithFuse;
                if (_hasFuse)
                {
                    SetPowered(startPowered);
                    IsPowered = startPowered;
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
                GameManager.Instance.SoundSystem.PlayOneShot(addFuseSound, AudioSource);
                _animator.SetTrigger(AddFuse);
                
                var inventory = GameManager.Instance.InventorySystem.PlayerInventory;
                inventory.UseItemInInventory(GetFirstFuse(inventory));
                inventory.SetSummaryEntryCompleted(summaryEntry);
                
                _hasFuse = true;
                GameManager.Instance.SaveSystem.SaveGame();
            }
            else
            {
                IsPowered = !IsPowered;
                SetPowered(IsPowered);
                GameManager.Instance.SoundSystem.PlayOneShot(IsPowered ? onSound : offSound, AudioSource);
            }
            
            return true;
        }
        
        private Item GetFirstFuse([NotNull] PlayerInventory inventory)
        {
            if (inventory == null) throw new ArgumentNullException(nameof(inventory));
            inventory = GameManager.Instance.InventorySystem.PlayerInventory;
            var itemsList = inventory.itemInventory;

            return fuse.FirstOrDefault(item => itemsList.Contains(item));
        }

        public void OnInteractFailed(GameObject player)
        {
            GameManager.Instance.SoundSystem.PlayOneShot(noFuseSound, AudioSource);
            
            if(!GameManager.Instance.InventorySystem.PlayerInventory.ContainsSummaryEntry(summaryEntry))
                GameManager.Instance.InventorySystem.PlayerInventory.AddSummaryEntry(summaryEntry);
        }

        public bool CanInteract()
        {
            var contains = GetFirstFuse(GameManager.Instance.InventorySystem.PlayerInventory) != null;
            
            return _hasFuse || contains;
        }

        //========================= Public methods =========================//

        public void SetPowered(bool state)
        {
            foreach (var powerObject in connectedObjects.Select(item => item.GetComponent<IPowered>() ?? item.GetComponentInChildren<IPowered>()))
            {
                if(state) powerObject.PowerOn();
                else powerObject.PowerOff();
            }
            
            PowerStateChanged?.Invoke(this, state);
            _animator.SetBool(Powered, state);
            _light.enabled = state;
            
            if(state) AudioSource.Play();
            else AudioSource.Stop();
        }
        
        public void SetFuseTrue()
        {
            _hasFuse = true;
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
                    IsPowered = false;
                    _animator.SetTrigger(NoFuse);
                }
                else
                {
                    _hasFuse = true;
                    IsPowered = fusebox == 1;
                    SetPowered(fusebox == 1);
                }
            }
            else
            {
                _hasFuse = startWithFuse;
                if (_hasFuse)
                {
                    IsPowered = startPowered;
                    SetPowered(startPowered);
                }
                else
                {
                    SetPowered(false);
                    IsPowered = false;
                    _animator.SetTrigger(NoFuse);
                }
            }
            
            return persistentID;
        }

        public void SaveData(SaveGame game)
        {
            if (_hasFuse) game.worldData.fuseBoxes[persistentID] = IsPowered ? 1 : 0;
            else game.worldData.fuseBoxes[persistentID] = 2;
        }
    }
}
