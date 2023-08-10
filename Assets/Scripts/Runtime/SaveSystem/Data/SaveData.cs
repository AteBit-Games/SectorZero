/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections.Generic;
using Runtime.InteractionSystem.Items;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using UnityEngine;

namespace Runtime.SaveSystem.Data
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new();
	
        [SerializeField]
        private List<TValue> values = new();
	
        // save the dictionary to lists
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach(var pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }
	
        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            this.Clear();

            if(keys.Count != values.Count)
                throw new Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

            for(int i = 0; i < keys.Count; i++) this.Add(keys[i], values[i]);
        }
    }
    
    [Serializable]
    public class MonsterSaveData
    {
        public bool isActive;
        public Vector3 position;
        public int activeState;
    }
    
    [Serializable]
    public class PlayerSaveData
    {
        public Vector3 position;
        public bool enabled = true;
        
        //Inventory
        public List<Tape> tapeInventory = new();
        public List<Item> itemInventory = new();
        public Throwable throwableItem;
    }

    [Serializable]
    public class TutorialData
    {
        public bool nellientState = true;
        public SerializableDictionary<string, bool> canvas = new();
    }

    [Serializable]
    public class WorldData
    {
        public bool nellientState = true;
        
        public SerializableDictionary<string, bool> pickups = new();
        public SerializableDictionary<string, bool> tapeRecorders = new();
        public SerializableDictionary<string, bool> triggers = new();
        public SerializableDictionary<string, bool> doors = new();
        
        public List<CinematicData> cinematics = new();
        public SerializableDictionary<string, bool> miscItems = new();
    }

    [Serializable]
    public class CinematicData
    {
        public string managerID;
        public List<bool> cinematicStates = new();
    }
    
    [Serializable]
    public class SaveData
    {
        public int currentScene;
        public PlayerSaveData playerData;
        public MonsterSaveData monsterData;
        public TutorialData tutorialData;
        public WorldData worldData;

        public SaveData() 
        {
            playerData = new PlayerSaveData();
            monsterData = new MonsterSaveData();
            tutorialData = new TutorialData();
            worldData = new WorldData();
        }
    }
    
}