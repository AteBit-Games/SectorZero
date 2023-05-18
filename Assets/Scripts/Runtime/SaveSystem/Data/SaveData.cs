/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections.Generic;
using Runtime.InteractionSystem;
using UnityEngine;

namespace Runtime.SaveSystem.Data
{
    [System.Serializable]
    public class SaveData
    {
        public int currentScene;
        public Vector3 playerPosition;

        public Dictionary<TapeRecorder, bool> tapeRecorders = new();

        public SaveData() 
        {
            playerPosition = new Vector3(1f, 40f, 0);
        }
    }
}