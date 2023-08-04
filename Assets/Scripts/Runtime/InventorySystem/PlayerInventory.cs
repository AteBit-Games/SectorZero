/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System.Collections.Generic;
using Runtime.InteractionSystem.Items;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using UnityEngine;

namespace Runtime.InventorySystem
{
    public enum ItemType
    {
        TapeRecording,
        Item
    }
    
    public class PlayerInventory : MonoBehaviour, IPersistant
    {
        // ReSharper disable once CollectionNeverQueried.Global
        public List<Tape> tapeInventory = new();
        public List<Item> itemInventory = new();
        public Throwable throwableItem;
        
        public bool HasThrowableItem => throwableItem != null;

        public bool AddTapeToInventory(Tape item)
        {
            tapeInventory.Add(item);
            return true;
        }
        
        public bool ContainsKeyItem(Item item)
        {
            return itemInventory.Contains(item);
        }
        
        public bool AddItemToInventory(Item item)
        {
            itemInventory.Add(item);
            return true;
        }
        
        public void UseItemInInventory(Item item)
        {
            itemInventory.Remove(item);
        }
        
        public void PickUpThrowable(GameObject item)
        {
            throwableItem = item.GetComponent<Throwable>();
        }
        
        public void DropThrowable()
        {
            throwableItem = null;
        }
        
        public Throwable GetThrowable()
        {
            return throwableItem;
        }

        public string ID { get; set; }
        public void LoadData(SaveData data)
        {
            itemInventory = data.playerData.itemInventory;
            tapeInventory = data.playerData.tapeInventory;
        }

        public void SaveData(SaveData data)
        {
            data.playerData.itemInventory = itemInventory;
            data.playerData.tapeInventory = tapeInventory;
        }
    }
}

