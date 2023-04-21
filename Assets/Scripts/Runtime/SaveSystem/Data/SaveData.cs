/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using UnityEngine;

namespace Runtime.SaveSystem.Data
{
    [System.Serializable]
    public class SaveData
    {
        public int currentScene;
        public Vector3 playerPosition;

        public SaveData() 
        {
            playerPosition = new Vector3((float)-24.1, (float)-21.6, 0);
        }
    }
}