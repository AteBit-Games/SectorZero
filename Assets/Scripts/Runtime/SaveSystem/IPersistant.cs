/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.SaveSystem.Data;

namespace Runtime.SaveSystem
{
    public interface IPersistant 
    {
        void LoadData(SaveGame game);
        void SaveData(SaveGame game);
    }
}