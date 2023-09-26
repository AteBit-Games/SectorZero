/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System.Collections.Generic;
using System.Linq;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Runtime.InventorySystem
{
    public class PlayerInventory : MonoBehaviour, IPersistant
    {
        public List<Tape> tapeInventory = new();
        public List<Item> itemInventory = new();
        public List<Note> noteInventory = new();
        
        public List<SummaryEntry> summaryEntries = new();
        private readonly List<SummaryEntry> _finishedSummaryRefs = new();
        
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
        
        public void AddSummaryEntry(SummaryEntry entry)
        {
            if(summaryEntries.Contains(entry) || _finishedSummaryRefs.Contains(entry)) return;
            
            Debug.Log("Adding summary entry");

            entry.isCompleted = false;
            summaryEntries.Add(entry);
            GameManager.Instance.NotificationManager.ShowSummaryAddedNotification();
        }
        
        public bool ContainsSummaryEntry(SummaryEntry entry)
        {
            return summaryEntries.Contains(entry);
        }
        
        public void SetSummaryEntryCompleted(SummaryEntry entry)
        {
            var index = summaryEntries.IndexOf(entry);
            if (index == -1)
            {
                Debug.Log("Summary Entry not found");
                _finishedSummaryRefs.Add(entry);
            }
            else
            {
                summaryEntries[index].isCompleted = true;
            }
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
            itemInventory.Clear();
            tapeInventory.Clear();
            noteInventory.Clear();
            
            foreach(var item in game.playerData.itemInventoryRefs)
            {
                Addressables.LoadAssetAsync<Item>(item).Completed += handle => itemInventory.Add(handle.Result);
            }
            
            foreach(var tape in game.playerData.tapeInventoryRefs)
            {
                Addressables.LoadAssetAsync<Tape>(tape).Completed += handle => tapeInventory.Add(handle.Result);
            }
            
            foreach(var note in game.playerData.noteInventoryRefs)
            {
                Addressables.LoadAssetAsync<Note>(note).Completed += handle => noteInventory.Add(handle.Result);
            }
            
            foreach(var entry in game.playerData.summaryEntries)
            {
                Addressables.LoadAssetAsync<SummaryEntry>(entry).Completed += handle => summaryEntries.Add(handle.Result);
            }
            
            return "Player Inventory";
        }

        public void SaveData(SaveGame game)
        {
            var itemRefs = itemInventory.Select(item => item.itemRef).ToList();
            game.playerData.itemInventoryRefs = itemRefs;

            var tapeRefs = tapeInventory.Select(tape => tape.itemRef).ToList();
            game.playerData.tapeInventoryRefs = tapeRefs;

            var noteRefs = noteInventory.Select(note => note.itemRef).ToList();
            game.playerData.noteInventoryRefs = noteRefs;

            var summaryRefs = summaryEntries.Select(entry => entry.itemRef).ToList();
            game.playerData.summaryEntries = summaryRefs;
        }
    }
}

