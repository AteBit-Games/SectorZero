/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;

namespace Runtime.SaveSystem.Data
{
    
    [Serializable]
    public class PlayerData
    {
        //Display
        public int displayMode = 1;
        public bool vSync = true;
        public float brightnessGain;

        //Audio
        public float masterVolume = 1f;
        public float musicVolume = 1f;
        public float voicesVolume = 1f;
        public float sfxVolume = 1f;
        
        //Gameplay
        public bool autoNotes = true;
        public bool autoTapes = true;
        public bool autoSkip = true;
        public int ending;
    }
}