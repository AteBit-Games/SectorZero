/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.InteractionSystem.Interfaces;
using Runtime.InventorySystem;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using UnityEngine;
using Tape = Runtime.InventorySystem.ScriptableObjects.Tape;

namespace Runtime.InteractionSystem.Items
{
    public class TapeRecorder : MonoBehaviour, IInteractable, IPersistant
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] public string persistentID;
        [SerializeField] private Tape tape;

        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            if (GameManager.Instance.SaveSystem.GetPlayerData().autoTapes)
            {
                GameManager.Instance.DialogueSystem.StartDialogue(tape.dialogue);
            }
            else
            {
                GameManager.Instance.NotificationManager.ShowPickupNotification(tape);
            }
            
            gameObject.SetActive(false);
            GameManager.Instance.SoundSystem.Play(interactSound);
            
            var inventory = player.GetComponentInParent<PlayerInventory>();
            return inventory.AddTapeToInventory(tape);
        }

        public void OnInteractFailed(GameObject player)
        {
            throw new System.NotImplementedException();
        }
        
        public bool CanInteract()
        {
            return true;
        }

        //========================= Save System =========================//
        
        public string LoadData(SaveGame game)
        {
            if (game.worldData.tapeRecorders.TryGetValue(persistentID, out var currentTape))
            {
                gameObject.SetActive(currentTape);
            }
            
            return persistentID;
        }
        
        public void SaveData(SaveGame game)
        {
            game.worldData.tapeRecorders[persistentID] = gameObject.activeSelf;
        }
    }
}
