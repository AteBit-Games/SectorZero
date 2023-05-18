/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.DialogueSystem;
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

        [SerializeField] private Item item;

        public bool OnInteract(GameObject player)
        {
            gameObject.SetActive(false);
            GameManager.Instance.SoundSystem.Play(interactSound, transform);
            var inventory = player.GetComponent<PlayerInventory>();
            return inventory.AddItemToInventory(item);
        }

        public void LoadData(SaveData data)
        {
            //data.tapeRecorders.Add(this, gameObject.activeSelf);
        }

        public void SaveData(SaveData data)
        {
            //data.tapeRecorders[this] = gameObject.activeSelf;
        }

        public bool CanInteract()
        {
            return true;
        }
    }
}
