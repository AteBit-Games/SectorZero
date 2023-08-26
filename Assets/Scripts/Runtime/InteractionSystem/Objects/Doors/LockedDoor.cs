/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using Runtime.InteractionSystem.Interfaces;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using Runtime.Utils;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects.Doors
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class LockedDoor : Door, IInteractable, IPersistant
    {
        [SerializeField] public string persistentID;
        [SerializeField] private Item key;

        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private Sound lockedSound;
        
        protected readonly int locked = Animator.StringToHash("locked");

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
            OpenDoor();
            
            //Remove the collider from the player's interactable list
            DisableInteraction();
            player.GetComponent<PlayerInteraction>().RemoveInteractable(gameObject);
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            GameManager.Instance.SoundSystem.Play(lockedSound, transform.GetComponent<AudioSource>());
            mainAnimator.SetTrigger(locked);
        }

        public bool CanInteract()
        {
            return GameManager.Instance.InventorySystem.PlayerInventory.ContainsKeyItem(key);
        }

        //=========================== Save System =============================//
        
        public void LoadData(SaveGame game)
        {
            if (game.worldData.doors.ContainsKey(persistentID))
            {
                if(game.worldData.doors[persistentID])
                {
                    mainAnimator.SetTrigger(Open);
                    mainAnimator.SetBool(isOpen, true);
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
                    mainAnimator.SetBool(isOpen, true);
                    DisableInteraction();
                    SetBlocker(0);
                    opened = true;
                }
            }
        }

        public void SaveData(SaveGame game)
        {
            if(!game.worldData.doors.ContainsKey(persistentID)) game.worldData.doors.Add(persistentID, opened);
            else game.worldData.doors[persistentID] = opened;
        }
    }
}
