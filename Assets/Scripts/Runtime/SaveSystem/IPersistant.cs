/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.SaveSystem.Data;

namespace Runtime.SaveSystem
{
    public interface IPersistant 
    {
        string LoadData(SaveGame game);
        void SaveData(SaveGame game);
    }
}