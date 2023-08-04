/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.SaveSystem.Data;
using UnityEngine;

namespace Runtime.SaveSystem
{
    public interface IPersistant 
    {
        public string ID { get; set; }
        void LoadData(SaveData data);
        void SaveData(SaveData data);
    }
}