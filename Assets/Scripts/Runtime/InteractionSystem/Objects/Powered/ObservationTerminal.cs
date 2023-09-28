/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
using System.Collections.Generic;
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
    public class ObservationTerminal : MonoBehaviour, IInteractable, IPowered, IPersistant
    {
        [SerializeField] private float triggerDuration;
        [SerializeField] public string persistentID;
        
        [SerializeField] private Sound offSound;
        [SerializeField] private Sound humSound;
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private List<PuzzleDoor> doors;
        [SerializeField] public SummaryEntry puzzleEntry;

        //----- Interface Properties -----//
        private bool _isPowered;
        public bool IsPowered { get; set; }

        //----- Private Variables -----//
        private Animator _animator;
        private AudioSource _audioSource;
        
        private bool _triggered;
        private bool _solved;
        private Coroutine _doorRoutine;
        private PlayerInventory _playerInventory;

        //----- Animator Hashes -----//
        private static readonly int Trigger = Animator.StringToHash("triggered");
        private static readonly int Powered = Animator.StringToHash("powered");
        private static readonly int Solved = Animator.StringToHash("solved");

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
            GameManager.Instance.SoundSystem.PlayOneShot(interactSound, _audioSource);
            _animator.SetTrigger(Trigger);
            
            _playerInventory = player.GetComponentInParent<PlayerInventory>();
            player.GetComponent<PlayerInteraction>().RemoveInteractable(gameObject);

            foreach (var door in doors)
            {
                door.TriggerDoor(triggerDuration);
            }
            
            _doorRoutine = StartCoroutine(TriggerDoors());
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            // switch (_isPowered)
            // {
            //     case false:
            //         GameManager.Instance.SoundSystem.PlayOneShot(offSound, _audioSource);
            //         if (!player.GetComponentInParent<PlayerInventory>().ContainsSummaryEntry(powerEntry))  
            //             GameManager.Instance.InventorySystem.PlayerInventory.AddSummaryEntry(powerEntry);
            //         break;
            //     case true when !GameManager.Instance.InventorySystem.PlayerInventory.ContainsKeyItem(keyCard):
            //         GameManager.Instance.SoundSystem.PlayOneShot(lockedSound, _audioSource);
            //         _animator.SetTrigger(Locked);
            //         if (!player.GetComponentInParent<PlayerInventory>().ContainsSummaryEntry(cardEntry))  
            //             GameManager.Instance.InventorySystem.PlayerInventory.AddSummaryEntry(cardEntry);
            //         break;
            // }
        }

        public bool CanInteract()
        {
            return _isPowered && !_triggered;
        }

        private IEnumerator TriggerDoors()
        {
            _triggered = true;
            _animator.SetTrigger(Trigger);
            
            yield return new WaitForSeconds(triggerDuration);
            
            if (!_playerInventory.ContainsSummaryEntry(puzzleEntry)) _playerInventory.AddSummaryEntry(puzzleEntry);
            _triggered = false;
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
            _audioSource.Stop();
            _animator.SetBool(Powered, false);
            
            if(_doorRoutine != null) StopCoroutine(_doorRoutine);
            if (_triggered && !_solved)
            {
                _playerInventory.SetSummaryEntryCompleted(puzzleEntry);
                DisableInteraction();
                _animator.SetTrigger(Solved);
                _solved = true;
            }
            
        }
        
        private void DisableInteraction()
        {
            gameObject.layer = 0;
            GetComponent<Collider2D>().enabled = false;
        }

        public string LoadData(SaveGame game)
        {
            if (game.worldData.containmentPuzzleComplete)
            {
                _solved = true;
                foreach (var door in doors)
                {
                    door.LoadDoor();
                }
                
                _animator.SetTrigger(Solved);
                DisableInteraction();
            }

            return persistentID;
        }

        public void SaveData(SaveGame game)
        {
            game.worldData.containmentPuzzleComplete = _solved;
        }
    }
}
