/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects.Doors
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class ElevatorDoor : Door, IInteractable, IPersistant
    {
        [SerializeField] public string persistentID;
        [SerializeField] private Sound interactSound;
        [SerializeField] private Sound interactFail;
        public Sound InteractSound => interactSound;

        public bool canInteract;

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
            GameManager.Instance.SaveSystem.SaveGame();
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            GameManager.Instance.SoundSystem.Play(interactFail, transform.GetComponent<AudioSource>());
        }

        public bool CanInteract()
        {
            return canInteract;
        }

        //=========================== Save System =============================//
        
        public string LoadData(SaveGame game)
        {
            if (game.worldData.doors.TryGetValue(persistentID, out var door))
            {
                if(door)
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
            
            return persistentID;
        }

        public void SaveData(SaveGame game)
        {
            game.worldData.doors[persistentID] = opened;
        }
    }
}
