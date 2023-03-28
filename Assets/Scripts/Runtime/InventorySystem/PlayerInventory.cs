using System;
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
        public readonly List<Tape> TapeInventory = new();
        public readonly List<Item> ItemInventory = new();

        public bool AddTapeToInventory(Tape item)
        {
            TapeInventory.Add(item);
            return true;
        }
        
        public bool AddItemToInventory(Item item)
        {
            ItemInventory.Add(item);
            return true;
        }

        
        private void RemoveItemFromInventory(Item item)
        {
            ItemInventory.Remove(item);
        }
    }
}

