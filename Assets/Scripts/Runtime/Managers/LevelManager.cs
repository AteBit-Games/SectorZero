/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using UnityEngine.SceneManagement;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.Managers
{
    public class LevelManager : MonoBehaviour
    {
        
        [SerializeField] private Sound ambiance;

        private void Awake()
        {
            SceneManager.sceneLoaded += PlayAmbiance;
        }

        private void PlayAmbiance(Scene arg0, LoadSceneMode arg1)
        {
            if (ambiance == null) return;
            GameManager.Instance.SoundSystem.Play(ambiance, transform, true);
        }
    }
}
