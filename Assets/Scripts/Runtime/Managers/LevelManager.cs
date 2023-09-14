/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using UnityEngine.SceneManagement;
using UnityEngine;
using NavMeshPlus.Components;
using Runtime.SoundSystem;

namespace Runtime.Managers
{
    [DefaultExecutionOrder(5)]
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private Sound startAmbience;
        [SerializeField] private bool playAmbienceOnStart;
        
        [SerializeField] private string levelDescription;
        [SerializeField] private NavMeshSurface navMeshSurface;
        
        private void Awake()
        {
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void Start()
        {
            if(navMeshSurface == null) return;
            navMeshSurface.hideEditorLogs = true;
            navMeshSurface.BuildNavMesh();
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GameManager.Instance.details = levelDescription;
            if(startAmbience != null && playAmbienceOnStart) GameManager.Instance.SoundSystem.FadeToNextAmbience(startAmbience);
        }
    }
}
