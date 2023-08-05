/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using UnityEngine;

namespace Runtime.Misc
{
    public class MiscPersistant : MonoBehaviour, IPersistant
    {
        [SerializeField] private string persistentID;
        public string ID
        {
            get => persistentID;
            set => persistentID = value;
        }
    
        public void LoadData(SaveData data)
        {
            if (data.worldData.miscItems.ContainsKey(persistentID))
            {
                gameObject.SetActive(data.worldData.miscItems[persistentID]);
            }
        }

        public void SaveData(SaveData data)
        {
            if(!data.worldData.miscItems.ContainsKey(persistentID)) data.worldData.miscItems.Add(persistentID, gameObject.activeSelf);
            else data.worldData.miscItems[persistentID] = gameObject.activeSelf;
        }
    }
}
