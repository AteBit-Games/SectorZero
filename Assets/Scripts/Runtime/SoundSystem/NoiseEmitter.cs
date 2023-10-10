/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Linq;
using Runtime.AI.Interfaces;
using UnityEngine;

namespace Runtime.SoundSystem
{
    public class NoiseEmitter : MonoBehaviour
    {
        [Header("EMITTER SETTINGS")]
        [SerializeField, Min(0), Tooltip("Noise max radius")] private float radius = 50f;

        [Header("LAYER MASK SETTINGS")]
        [SerializeField, Tooltip("Objects capable of hearing noise")] private LayerMask targetMask;
        
        //----- Interface -----//
        public float Radius
        {
            get => radius;
            set => radius = value;
        }
        
        // ================================ Public Methods ================================
        
        public void EmitLocal()
        {
            var targetsInRadius = Physics2D.OverlapCircleAll(transform.position, radius, targetMask);
            foreach (var t in targetsInRadius)
            {
                var wallMask = LayerMask.GetMask("Walls");
                var ray = Physics2D.Raycast(transform.position,
                    t.transform.position - transform.position, radius, targetMask | wallMask);
                if (ray.collider != null)
                {
                    var lineOfSight = (targetMask.value & 1 << ray.transform.gameObject.layer) > 0 && ray.collider.CompareTag("Monster");
                    if (lineOfSight)
                    {
                        var hearing = t.GetComponentInChildren<IHearingHandler>();
                        hearing?.OnHearing(this);
                    }
                    else if(Random.Range(0, 100) < 20)
                    {
                        var hearing = t.GetComponentInChildren<IHearingHandler>();
                        hearing?.OnHearing(this);
                    }
                }
            }
        }
        
        public void EmitGlobal()
        {
            var targets = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IHearingHandler>();
            foreach (var t in targets)
            {
                t.OnHearing(this);
            }
        }

        // ================================== Unity Gizmos =====================================
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.15f, 0.67f, 0.88f);
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}