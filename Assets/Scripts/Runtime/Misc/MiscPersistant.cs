/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using UnityEngine;

namespace Runtime.Misc
{
    /// <summary>
    /// Used on items that need to save their enabled state
    /// </summary>
    public class MiscPersistant : MonoBehaviour, IPersistant
    {
        [SerializeField] public string persistentID;

        //============================== Save System ==============================
        
        public string LoadData(SaveGame game)
        {
            if (game.worldData.miscItems.TryGetValue(persistentID, out var item))
            {
                gameObject.SetActive(item);
            }
            
            return persistentID;
        }

        public void SaveData(SaveGame game)
        {
            game.worldData.miscItems[persistentID] = gameObject.activeSelf;
        }
    }
}
