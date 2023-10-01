/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.InteractionSystem.Interfaces;
using Runtime.InventorySystem;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects.Doors
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class LockedDoor : Door, IInteractable, IPersistant, IPowered
    {
        [SerializeField] public string persistentID;
        [SerializeField] private Item key;
        [SerializeField] private bool destroyKeyOnUse;

        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private Sound lockedSound;
        [SerializeField] private Sound poweredOffSound;
        
        [SerializeField] public bool addsSummaryOnFail;
        [SerializeField] public bool finishSummaryOnOpen;
        [SerializeField] private SummaryEntry failSummaryEntry;
        [SerializeField] private SummaryEntry openSummaryEntry;

        [SerializeField] public bool saveOnOpen;
        
        public bool IsPowered { get; set; }
        private readonly int _locked = Animator.StringToHash("locked");
        private readonly int _poweredOff = Animator.StringToHash("poweredOff");

        //=========================== Unity Events =============================//

        private void Start()
        {
            if(GameManager.Instance.TestMode && startOpen)
            {
                mainAnimator.SetTrigger(Open);
                mainAnimator.SetBool(isOpen, true);
                
                DisableInteraction();
                SetBlocker(0);
                opened = true;
            }
        }

        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.SoundSystem.Play(interactSound, transform.GetComponent<AudioSource>());
            if(destroyKeyOnUse) player.GetComponentInParent<PlayerInventory>().UseItemInInventory(key);
            OpenDoor();

            if (finishSummaryOnOpen)
            {
                GameManager.Instance.InventorySystem.PlayerInventory.SetSummaryEntryCompleted(openSummaryEntry);
            }
            if(saveOnOpen) GameManager.Instance.SaveSystem.SaveGame();
            
            //Remove the collider from the player's interactable list
            DisableInteraction();
            player.GetComponent<PlayerInteraction>().RemoveInteractable(gameObject);
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            if (IsPowered)
            {
                GameManager.Instance.SoundSystem.Play(lockedSound, transform.GetComponent<AudioSource>());
                mainAnimator.SetTrigger(_locked);
            }
            else
            {
                GameManager.Instance.SoundSystem.Play(poweredOffSound, transform.GetComponent<AudioSource>());
            }
            
            if(addsSummaryOnFail)
            { 
                var inventory = player.GetComponentInParent<PlayerInventory>();
                if(!inventory.ContainsSummaryEntry(failSummaryEntry)) inventory.AddSummaryEntry(failSummaryEntry);
            }
        }

        public bool CanInteract()
        {
            return GameManager.Instance.InventorySystem.PlayerInventory.ContainsKeyItem(key) && IsPowered;
        }
        
        public void PowerOn(bool load)
        {
            mainAnimator.SetBool(_poweredOff, false);
            IsPowered = true;
        }
        
        public void PowerOff()
        {
            mainAnimator.SetBool(_poweredOff, true);
            IsPowered = false;
        }

        //=========================== Save System =============================//
        
        public string LoadData(SaveGame game)
        {
            if (game.worldData.doors.TryGetValue(persistentID, out var door))
            {
                if(door)
                {
                    mainAnimator.SetTrigger(Open);
                    DisableInteraction();
                    SetBlocker(0);
                    opened = true;
                }
            }
            else
            {
                if(startOpen)
                {
                    mainAnimator.SetTrigger(Open);
                    DisableInteraction();
                    SetBlocker(0);
                    opened = true;
                }
            }
            
            return persistentID;
        }

        public void SaveData(SaveGame game)
        {
            game.worldData.doors[persistentID] = opened;
        }
    }
}
