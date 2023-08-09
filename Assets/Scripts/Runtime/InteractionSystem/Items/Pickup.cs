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
        
        [SerializeField] private string persistentID;
        public string ID
        {
            get => persistentID;
            set => persistentID = value;
        }

        [SerializeField] private Item item;

        public bool OnInteract(GameObject player)
        {
            gameObject.SetActive(false);
            GameManager.Instance.SoundSystem.Play(interactSound, transform.GetComponent<AudioSource>());
            var inventory = player.GetComponentInParent<PlayerInventory>();
            GameManager.Instance.NotificationManager.ShowPickupNotification(item);
            return inventory.AddItemToInventory(item);
        }

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
        
        public bool CanInteract()
        {
            return true;
        }

        public UnityEvent OnInteractEvents { get; }
        public UnityEvent OnInteractFailedEvents { get; }
        public bool failedToInteract { get; set; }
    }
}
