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
    // [Serializable]
    // public class CullingSection
    // {
    //     public string tag;
    // }
    //
    // [Serializable]
    // public class CasterCullingSection : CullingSection
    // {
    //     public Transform targetCenter;
    //     public float radius;
    //     public GameObject cullingCasterContainer;
    // }
    
    [DefaultExecutionOrder(5)]
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private Sound startAmbience;
        [SerializeField] private bool playAmbienceOnStart;
        
        [SerializeField] private string levelDescription;
        [SerializeField] private NavMeshSurface navMeshSurface;
        // [SerializeField] private Transform cullingReferencePoint;
        //
        // [SerializeField] private bool showCasterTargets;
        // public List<CasterCullingSection> casterTargets;
        //
        // private CullingGroup _cullingGroup;
        // private BoundingSphere[] _bounds;
        // private Dictionary<int, CullingSection> _cullingTargets;

        private void Awake()
        {
            // _cullingGroup = new CullingGroup();
            //
            // _cullingGroup.targetCamera = Camera.main;
            // _cullingGroup.SetDistanceReferencePoint(cullingReferencePoint);
            // _cullingGroup.SetBoundingDistances(new[] { 25.0f });
            //
            // var count = casterTargets.Count;
            // _cullingTargets = new Dictionary<int, CullingSection>(count);
            //
            // var index = 0;
            // _bounds = new BoundingSphere[count];
            //
            // foreach (var section in casterTargets)
            // {
            //     _bounds[index] = new BoundingSphere(section.targetCenter.position, section.radius);
            //     _cullingTargets.Add(index, section);
            //     index++;
            // }
            //
            // _cullingGroup.SetBoundingSpheres(_bounds);
            // _cullingGroup.SetBoundingSphereCount(count);
            //
            // foreach (var section in _cullingTargets.Values)
            // {
            //     if(section is CasterCullingSection castSection)
            //     {
            //         var casters = castSection.cullingCasterContainer.gameObject.GetComponentsInChildren<ShadowCaster2D>();
            //         foreach (var caster in casters)
            //         {
            //             caster.castingOption = (ShadowCaster2D.ShadowCastingOptions)ShadowCastingMode.Off;
            //         }
            //     }
            // }
            //
            // _cullingGroup.onStateChanged = OnChange;
            SceneManager.sceneLoaded += SceneLoaded;
        }
        
        // private void OnChange(CullingGroupEvent group)
        // {
        //     if(group.hasBecomeVisible)
        //     {
        //         var target = _cullingTargets[group.index];
        //         if(target is CasterCullingSection castSection)
        //         {
        //             var casters = castSection.cullingCasterContainer.gameObject.GetComponentsInChildren<ShadowCaster2D>();
        //             foreach (var caster in casters)
        //             {
        //                 caster.castingOption = (ShadowCaster2D.ShadowCastingOptions)ShadowCastingMode.TwoSided;
        //             }
        //         }
        //     }
        //     else if(group.hasBecomeInvisible)
        //     {
        //         var target = _cullingTargets[group.index];
        //         if(target is CasterCullingSection castSection)
        //         {
        //             var casters = castSection.cullingCasterContainer.gameObject.GetComponentsInChildren<ShadowCaster2D>();
        //             foreach (var caster in casters)
        //             {
        //                 caster.castingOption = (ShadowCaster2D.ShadowCastingOptions)ShadowCastingMode.Off;
        //             }
        //         }
        //     }
        // }

        private void Start()
        {
            if(navMeshSurface == null) return;
            navMeshSurface.hideEditorLogs = true;
            navMeshSurface.BuildNavMesh();
        }

        // private void OnDestroy()
        // {
        //     _cullingGroup.Dispose();
        //     _cullingGroup = null;
        // }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GameManager.Instance.details = levelDescription;
            if(startAmbience != null && playAmbienceOnStart) GameManager.Instance.SoundSystem.FadeToNextAmbience(startAmbience);
        }

        //
        // private void OnDrawGizmosSelected()
        // {
        //     if (showCasterTargets)
        //     {
        //         foreach (var section in casterTargets.Where(section => section.targetCenter != null))
        //         {
        //             Gizmos.color = Color.cyan;
        //             Gizmos.DrawWireSphere(section.targetCenter.position, section.radius);
        //         }
        //     }
        // }
    }
}
