/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System.Collections.Generic;
using Runtime.InventorySystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.InventorySystem
{
    public enum ItemType
    {
        TapeRecording,
        Item
    }
    
    public class PlayerInventory : MonoBehaviour
    {
        // ReSharper disable once CollectionNeverQueried.Global
        public readonly List<Tape> tapeInventory = new();
        public readonly List<Item> itemInventory = new();

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
    }
}

