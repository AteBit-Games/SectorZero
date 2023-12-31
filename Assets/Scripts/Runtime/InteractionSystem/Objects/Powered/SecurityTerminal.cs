/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.InteractionSystem.Interfaces;
using Runtime.InteractionSystem.Objects.Doors;
using Runtime.InventorySystem;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects.Powered
{
    [DefaultExecutionOrder(6)]
    public class SecurityTerminal : MonoBehaviour, IInteractable, IPowered, IPersistant
    {
        [SerializeField] private Sound offSound;
        [SerializeField] private Sound lockedSound;
        [SerializeField] private Sound humSound;
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private Item keyCard;
        [SerializeField] private TriggerDoor door;
        [SerializeField] public NoiseEmitter noiseEmitter;
        [SerializeField] public SummaryEntry powerEntry;
        [SerializeField] public SummaryEntry cardEntry;

        //----- Interface Properties -----//
        private bool _isPowered;
        private bool _isOpen;

        public bool IsPowered { get; set; }

        //----- Private Variables -----//
        private Animator _animator;
        private AudioSource _audioSource;
        
        //----- Animator Hashes -----//
        private static readonly int Powered = Animator.StringToHash("powered");
        private static readonly int Unlock = Animator.StringToHash("unlock");
        private static readonly int Loaded = Animator.StringToHash("loaded");
        private static readonly int Locked = Animator.StringToHash("locked");

        //========================= Unity events =========================//

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
            GameManager.Instance.SoundSystem.SetupSound(_audioSource, offSound);
        }

        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.SoundSystem.PlayOneShot(offSound, _audioSource);
            _animator.SetTrigger(Unlock);
            _isOpen = true;
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            switch (_isPowered)
            {
                case false:
                    GameManager.Instance.SoundSystem.PlayOneShot(offSound, _audioSource);
                    if (!player.GetComponentInParent<PlayerInventory>().ContainsSummaryEntry(powerEntry))  
                        GameManager.Instance.InventorySystem.PlayerInventory.AddSummaryEntry(powerEntry);
                    break;
                case true when !GameManager.Instance.InventorySystem.PlayerInventory.ContainsKeyItem(keyCard):
                    GameManager.Instance.SoundSystem.PlayOneShot(lockedSound, _audioSource);
                    _animator.SetTrigger(Locked);
                    if (!player.GetComponentInParent<PlayerInventory>().ContainsSummaryEntry(cardEntry))  
                        GameManager.Instance.InventorySystem.PlayerInventory.AddSummaryEntry(cardEntry);
                    break;
            }
        }

        public bool CanInteract()
        {
            return _isPowered && GameManager.Instance.InventorySystem.PlayerInventory.ContainsKeyItem(keyCard);
        }

        public void TriggerDoor()
        {
            door.OpenDoor();
            noiseEmitter.EmitGlobal();
            GameManager.Instance.SaveSystem.SaveGame();
        }

        //========================= Public methods =========================//
        
        public void PowerOn(bool load)
        {
            _isPowered = true;
            _animator.SetBool(Powered, true);
            GameManager.Instance.SoundSystem.Play(humSound, _audioSource);
        }

        public void PowerOff()
        {
            _isPowered = false;
            _animator.SetBool(Powered, false);
            _audioSource.Stop();
        }

        public string LoadData(SaveGame game)
        {
            if (game.worldData.securityTerminalUnlocked)
            {
                _isOpen = true;
                _animator.SetTrigger(Loaded);
            }

            return "SecurityTerminal";
        }

        public void SaveData(SaveGame game)
        {
            game.worldData.securityTerminalUnlocked = _isOpen;
        }
    }
}
