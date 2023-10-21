/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections.Generic;
using Runtime.Misc.Triggers;
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
        public Vector3 position = new(-33f, -0.65f, 0);
        public int activeState;
        public float menaceGaugeValue = 50;
        public bool menaceState;
        public int aggroLevel;
        public float lastSeenPlayerTime;

        public bool addedInitialSentinels;
    }
    
    [Serializable]
    public class PlayerSaveData
    {
        public string activeWing = "MedicalWing";
        public Vector3 position;
        public bool enabled;
        
        //Inventory
        public List<string> tapeInventoryRefs = new();
        public List<string> itemInventoryRefs = new();
        public List<string> noteInventoryRefs = new();
        
        public List<string> summaryEntries = new();
        public List<string> summaryEntriesFinished = new();
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
        public bool safeOpen;
        public bool containmentPuzzleComplete;
        
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
        public bool isDataSaved;
        public long saveTime;
        public string levelName;
        
        public PlayerSaveData playerData = new();
        public MonsterSaveData monsterData = new();
        
        public TutorialData tutorialData = new();
        public WorldData worldData = new();
    }
}