/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.DialogueSystem;
using Runtime.InteractionSystem.Interfaces;
using Runtime.InventorySystem;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.InteractionSystem.Items
{
    public class Pickup : MonoBehaviour, IInteractable, IPersistant
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] public string persistentID;
        [SerializeField] private Item item;

        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            gameObject.SetActive(false);
            GameManager.Instance.SoundSystem.Play(interactSound, transform.GetComponent<AudioSource>());
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

        public void LoadData(SaveData data)
        {
            if (data.worldData.pickups.ContainsKey(persistentID))
            {
                gameObject.SetActive(data.worldData.pickups[persistentID]);
            }
        }

        public void SaveData(SaveData data)
        {
            if(!data.worldData.pickups.ContainsKey(persistentID)) data.worldData.pickups.Add(persistentID, gameObject.activeSelf);
            else data.worldData.pickups[persistentID] = gameObject.activeSelf;
        }
    }
}
