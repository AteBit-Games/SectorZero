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
    }

    [Serializable]
    public class LightCullingSection : CullingSection
    {
        public List<CullingLight> cullingLightTargets;
    }
    
    [Serializable]
    public class WallCullingSection : CullingSection
    {
        public Transform targetCenter;
        public float radius;
        public GameObject cullingWallContainer;
    }

    [Serializable]
    public class CullingLight
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
        public List<LightCullingSection> lightTargets;
        public List<WallCullingSection> wallTargets;

        private CullingGroup cullingGroup;
        private BoundingSphere[] bounds;
        private Dictionary<int, CullingSection> cullingTargets;

        private void Awake()
        {
            cullingGroup = new CullingGroup();

            cullingGroup.targetCamera = Camera.main;
            cullingGroup.SetDistanceReferencePoint(cullingReferencePoint);
            cullingGroup.SetBoundingDistances(new[] { 25.0f });

            int count = lightTargets.Sum(section => section.cullingLightTargets.Count);
            count += wallTargets.Count;
            cullingTargets = new Dictionary<int, CullingSection>(count);

            int index = 0;
            bounds = new BoundingSphere[count];
            foreach (var section in lightTargets)
            {
                foreach (var target in section.cullingLightTargets)
                {
                    bounds[index] = new BoundingSphere(target.targetCenter.position, target.radius);
                    cullingTargets.Add(index, section);
                    index++;
                }
            }
            

            foreach (var section in wallTargets)
            {
                bounds[index] = new BoundingSphere(section.targetCenter.position, section.radius);
                cullingTargets.Add(index, section);
                index++;
            }

            cullingGroup.SetBoundingSpheres(bounds);
            cullingGroup.SetBoundingSphereCount(count);

            foreach (var section in cullingTargets.Values)
            {
                if(section is LightCullingSection lightSection)
                {
                    foreach (var customLight in lightSection.cullingLightTargets.SelectMany(target => target.lights))
                    {
                        customLight.gameObject.SetActive(false);
                    }
                }
                else if(section is WallCullingSection wallSection)
                {
                    for (var i = 0; i < wallSection.cullingWallContainer.gameObject.transform.childCount; i++)
                    {
                        var wall = wallSection.cullingWallContainer.gameObject.transform.GetChild(i);
                        wall.gameObject.SetActive(false);
                    }
                }
            }

            cullingGroup.onStateChanged = OnChange;
            SceneManager.sceneLoaded += SceneLoaded;
        }
        
        private void OnChange(CullingGroupEvent group)
        {
            if(group.hasBecomeVisible)
            {
                var target = cullingTargets[group.index];
                if(target is LightCullingSection lightSection)
                {
                    Debug.Log($"Target {target} is a light section");
                    foreach (var customLight in lightSection.cullingLightTargets.SelectMany(cullingLight => cullingLight.lights))
                    {
                        customLight.gameObject.SetActive(true);
                    }
                }
                else if(target is WallCullingSection wallSection)
                {
                    for (var i = 0; i < wallSection.cullingWallContainer.gameObject.transform.childCount; i++)
                    {
                        var wall = wallSection.cullingWallContainer.gameObject.transform.GetChild(i);
                        wall.gameObject.SetActive(true);
                    }
                }
            }
            else if(group.hasBecomeInvisible)
            {
                var target = cullingTargets[group.index];
                if(target is LightCullingSection lightSection)
                {
                    foreach (var customLight in lightSection.cullingLightTargets.SelectMany(cullingLight => cullingLight.lights))
                    {
                        customLight.gameObject.SetActive(false);
                    }
                }
                else if(target is WallCullingSection wallSection)
                {
                    for (var i = 0; i < wallSection.cullingWallContainer.gameObject.transform.childCount; i++)
                    {
                        var wall = wallSection.cullingWallContainer.gameObject.transform.GetChild(i);
                        wall.gameObject.SetActive(false);
                    }
                }
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

        
        private readonly Color[] colors = {Color.red, Color.blue, Color.green, Color.yellow, Color.cyan, Color.magenta};
        private void OnDrawGizmos()
        {
            
            // if (lightTargets == null) return;
            //
            // var index = 0;
            // foreach (var section in lightTargets)
            // {
            //     Gizmos.color = colors[index];
            //     foreach (var target in section.cullingLightTargets)
            //     {
            //         Gizmos.DrawWireSphere(target.targetCenter.position, target.radius);
            //     }
            //     index++;
            //}
            
            // foreach (var section in wallTargets)
            // {
            //     Gizmos.color = Color.white;
            //     Gizmos.DrawWireSphere(section.targetCenter.position, section.radius);
            // }
        }
    }
}
