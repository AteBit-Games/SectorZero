/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections.Generic;
using Runtime.InteractionSystem.Items;
using Runtime.InventorySystem.ScriptableObjects;
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
            Clear();

            if(keys.Count != values.Count)
                throw new Exception(
                    $"there are {keys.Count} keys and {values.Count} values after deserialization. Make sure that both key and value types are serializable.");

            for(int i = 0; i < keys.Count; i++) Add(keys[i], values[i]);
        }
    }
    
    [Serializable]
    public class CinematicData
    {
        public string managerID;
        public List<bool> cinematicStates = new();
    }
    
    [Serializable]
    public class MonsterSaveData
    {
        public bool isActive;
        public Vector3 position;
        public int activeState;
        public float menaceGaugeValue;
        public bool menaceState;
        public int aggroLevel;
        public float lastSeenPlayerTime;
    }
    
    [Serializable]
    public class PlayerSaveData
    {
        public Vector3 position;
        public bool enabled;
        
        //Inventory
        public List<Tape> tapeInventory = new();
        public List<Item> itemInventory = new();
        public List<Note> noteInventory = new();
    }

    [Serializable]
    public class TutorialData
    {
        public bool nellientState = true;
        public SerializableDictionary<string, bool> canvas = new();
        public List<CinematicData> tutorialCinematics = new();
    }

    [Serializable]
    public class WorldData
    {
        public bool nellientState = true;
        public bool mainFuseStatus = true;
        
        public SerializableDictionary<string, bool> pickups = new();
        public SerializableDictionary<string, bool> tapeRecorders = new();
        public SerializableDictionary<string, bool> notes = new();
        
        public SerializableDictionary<string, bool> triggers = new();
        public SerializableDictionary<string, bool> doors = new();
        public SerializableDictionary<string, bool> miscItems = new();
        public SerializableDictionary<string, int> fuseBoxes = new();
        
        public List<CinematicData> cinematics = new();
    }

    [Serializable]
    public class SaveGame
    {
        public string saveName;
        public long saveTime;
        public int currentScene;
        
        public PlayerSaveData playerData = new();
        public SerializableDictionary<string, MonsterSaveData> monsterData = new();
        
        public TutorialData tutorialData = new();
        public WorldData worldData = new();
        
        public SaveGame()
        {
            monsterData.Add("VoidMask", new MonsterSaveData
            {
                isActive = true,
                position = new Vector3(-33f, -0.65f, 0),
                activeState = 0,
                menaceGaugeValue = 45f,
                menaceState = false,
                aggroLevel = 0,
                lastSeenPlayerTime = 0
            });
        }
    }

    [Serializable]
    public class PlayerData
    {
        public int displayMode = 1;
        public bool vSync = true;
        public bool autoNotes = true;
        public bool autoTapes = true;
        public float masterVolume = 1f;
        public float musicVolume = 1f;
        public float sfxVolume = 1f;
    }
}