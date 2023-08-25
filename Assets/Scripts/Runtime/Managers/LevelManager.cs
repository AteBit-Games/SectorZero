/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using NavMeshPlus.Components;
using Runtime.SoundSystem;

namespace Runtime.Managers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private string levelDescription;
        [SerializeField] private NavMeshSurface navMeshSurface;

        private  CullingGroup _cullingGroup;
        private BoundingSphere[] _boundingSpheres;
        private float _playerRadius = 1f;
        
        private void Awake()
        {
            SceneManager.sceneLoaded += SceneLoaded;
            _cullingGroup = new CullingGroup();
            _cullingGroup.targetCamera = mainCamera;
            _boundingSpheres = new BoundingSphere[25];
            _boundingSpheres[0] = new BoundingSphere(Vector3.zero, 10f);
            _cullingGroup.SetBoundingSpheres(_boundingSpheres);
            _cullingGroup.onStateChanged = StateChangedMethod;
        }
        
        private void StateChangedMethod(CullingGroupEvent evt)
        {
            if(evt.hasBecomeVisible)
                Debug.LogFormat("Sphere {0} has become visible!", evt.index);
            if(evt.hasBecomeInvisible)
                Debug.LogFormat("Sphere {0} has become invisible!", evt.index);
        }

        private void Start()
        {
            if(navMeshSurface == null) return;
            navMeshSurface.hideEditorLogs = true;
            navMeshSurface.BuildNavMesh();
        }

        private void OnDestroy()
        {
            _cullingGroup.Dispose();
        }

        private void Update()
        {
            
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GameManager.Instance.details = levelDescription;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Vector3.zero, 10f);
        }
    }
}
