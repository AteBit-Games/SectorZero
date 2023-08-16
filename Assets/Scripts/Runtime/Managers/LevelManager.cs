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
    public class LevelManager : MonoBehaviour
    {
        
        [SerializeField] private Sound ambiance;
        [SerializeField] private string levelDescription;
        [SerializeField] private NavMeshSurface navMeshSurface;

        private void Awake()
        {
            SceneManager.sceneLoaded += PlayAmbiance;
        }

        private void Start()
        {
            if(navMeshSurface == null) return;
            navMeshSurface.hideEditorLogs = true;
            navMeshSurface.BuildNavMesh();
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= PlayAmbiance;
        }

        private void PlayAmbiance(Scene arg0, LoadSceneMode arg1)
        {
            GameManager.Instance.details = levelDescription;
            if (ambiance == null) return;
        }
    }
}
