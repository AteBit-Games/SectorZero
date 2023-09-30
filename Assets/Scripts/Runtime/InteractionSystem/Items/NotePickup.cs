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

namespace Runtime.InteractionSystem.Items
{
    public class NotePickup : MonoBehaviour, IInteractable, IPersistant
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] public string persistentID;
        [SerializeField] private Note note;

        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.SoundSystem.PlayRealtime(interactSound);
            
            if(GameManager.Instance.SaveSystem.GetPlayerData().autoNotes)
            {
                GameManager.Instance.HUD.OpenNote(note, true);
            }
            else
            {
                GameManager.Instance.NotificationManager.ShowPickupNotification(note);
            }
            
            gameObject.SetActive(false);
            
            var inventory = player.GetComponentInParent<PlayerInventory>();
            return inventory.AddNoteToInventory(note);
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
            if (game.worldData.notes.TryGetValue(persistentID, out var currentNote))
            {
                gameObject.SetActive(currentNote);
            }
            
            return persistentID;
        }
        
        public void SaveData(SaveGame game)
        {
            game.worldData.notes[persistentID] = gameObject.activeSelf;
        }
    }
}
