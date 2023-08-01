/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections.Generic;
using Runtime.InteractionSystem.Items;
using UnityEngine;

namespace Runtime.SaveSystem.Data
{
    [Serializable]
    public class MonsterSaveData
    {
        public bool isActive;
        public Vector3 position;
        public int activeState;
    }
    
    [Serializable]
    public class PlayerSaveData
    {
        public Vector3 position;

        public PlayerSaveData()
        {
            position = new Vector3(-0.54f, 7.25f, 0);
        }
    }

    [Serializable]
    public class TutorialData
    {
        public bool isTutorialLevel;
        public int activeStage;
    }
    
    [Serializable]
    public class SaveData
    {
        public int currentScene;
        public PlayerSaveData playerData;
        public MonsterSaveData monsterData;
        public TutorialData tutorialData;
        
        public Dictionary<TapeRecorder, bool> tapeRecorders = new();

        public SaveData() 
        {
            playerData = new PlayerSaveData();
            monsterData = new MonsterSaveData();
            tutorialData = new TutorialData();
        }
    }
    
}