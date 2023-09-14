/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System.Collections.Generic;
using System.Linq;
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
        Item,
        Note
    }
    
    public class PlayerInventory : MonoBehaviour, IPersistant
    {
        public List<Tape> tapeInventory = new();
        public List<Item> itemInventory = new();
        public List<Note> noteInventory = new();
        
        
        public bool AddTapeToInventory(Tape item)
        {
            tapeInventory.Add(item);
            return true;
        }
        
        public bool AddNoteToInventory(Note item)
        {
            noteInventory.Add(item);
            return true;
        }
        
        public bool AddItemToInventory(Item item)
        {
            itemInventory.Add(item);
            return true;
        }
        
        public bool ContainsKeyItem(Item item)
        {
            return itemInventory.Contains(item);
        }
        
        public void UseItemInInventory(Item item)
        {
            itemInventory.Remove(item);
        }
        
        //=============================== Save System ===============================//
        
        public string LoadData(SaveGame game)
        {
            
            
            //itemInventory = game.playerData.itemInventory;
            //tapeInventory = game.playerData.tapeInventory;
            //noteInventory = game.playerData.noteInventory;
            
            return "Player Inventory";
        }

        public void SaveData(SaveGame game)
        {
            var itemRefs = itemInventory.Select(item => item.itemRef.ToString()).ToList();

            game.playerData.itemInventoryRefs = itemRefs;
            // game.playerData.tapeInventory = tapeInventory;
            // game.playerData.noteInventory = noteInventory;
        }
    }
}

