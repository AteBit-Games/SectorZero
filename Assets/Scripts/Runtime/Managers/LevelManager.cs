/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine;
using NavMeshPlus.Components;
using Runtime.Misc;

namespace Runtime.Managers
{
    [Serializable]
    public class CullingSection
    {
        public string tag;
        public List<CullingTarget> cullingTargets;
    }

    [Serializable]
    public class CullingTarget
    {
        public Transform targetCenter;
        public float radius;
        public List<CustomLight> lights;
    }
    
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private string levelDescription;
        [SerializeField] private NavMeshSurface navMeshSurface;
        [SerializeField] private Transform cullingReferencePoint;
        public List<CullingSection> targets;

        private CullingGroup cullingGroup;
        private BoundingSphere[] bounds;
        private CullingTarget[] cullingTargets;

        
        private void Awake()
        {
            cullingGroup = new CullingGroup();

            cullingGroup.targetCamera = Camera.main;
            cullingGroup.SetDistanceReferencePoint(cullingReferencePoint);
            cullingGroup.SetBoundingDistances(new[] { 10.0f });

            int count = targets.Sum(section => section.cullingTargets.Count);

            int index = 0;
            bounds = new BoundingSphere[count];
            cullingTargets = new CullingTarget[count];
            foreach (var target in targets.SelectMany(section => section.cullingTargets))
            {
                bounds[index] = new BoundingSphere(target.targetCenter.position, target.radius);
                cullingTargets[index] = target;
                index++;
            }

            cullingGroup.SetBoundingSpheres(bounds);
            cullingGroup.SetBoundingSphereCount(count);

            foreach (var customLight in targets.SelectMany(section => section.cullingTargets.SelectMany(target => target.lights)))
            {
                customLight.gameObject.SetActive(false);
            }

            cullingGroup.onStateChanged = OnChange;
            SceneManager.sceneLoaded += SceneLoaded;
        }
        
        private void OnChange(CullingGroupEvent group)
        {
            if(group.hasBecomeVisible)
            {
                foreach (var customLight in cullingTargets[group.index].lights) customLight.gameObject.SetActive(true);
            }
            else if(group.hasBecomeInvisible)
            {
                foreach (var customLight in cullingTargets[group.index].lights) customLight.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            if(navMeshSurface == null) return;
            navMeshSurface.hideEditorLogs = true;
            navMeshSurface.BuildNavMesh();
        }

        private void OnDestroy()
        {
            cullingGroup.Dispose();
            cullingGroup = null;
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GameManager.Instance.details = levelDescription;
        }

        private void OnDrawGizmos()
        {
            if (targets == null) return;
            Gizmos.color = Color.red;
            for (int i = 0; i < targets.Count; i++)
            {
                foreach (var target in targets[i].cullingTargets)
                {
                    if(target.targetCenter == null) continue;
                    Gizmos.DrawWireSphere(target.targetCenter.position, target.radius);
                }
            }
        }
    }
}
