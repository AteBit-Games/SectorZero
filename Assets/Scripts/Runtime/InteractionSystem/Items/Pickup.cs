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
    public class Pickup : MonoBehaviour, IInteractable, IPersistant
    {
        [SerializeField] private bool saveOnPickup;
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] public string persistentID;
        [SerializeField] private Item item;
        
        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            if(saveOnPickup) GameManager.Instance.SaveSystem.SaveGame();
            
            gameObject.SetActive(false);
            GameManager.Instance.SoundSystem.Play(interactSound);
            var inventory = player.GetComponentInParent<PlayerInventory>();
            
            GameManager.Instance.NotificationManager.ShowPickupNotification(item);
            return inventory.AddItemToInventory(item);
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
            if (game.worldData.pickups.TryGetValue(persistentID, out var pickup))
            {
                gameObject.SetActive(pickup);
            }
            
            return persistentID;
        }

        public void SaveData(SaveGame game)
        {
            game.worldData.pickups[persistentID] = gameObject.activeSelf;
        }
    }
}
