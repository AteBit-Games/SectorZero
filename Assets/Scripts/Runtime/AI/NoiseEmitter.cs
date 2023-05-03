/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.AI.Interfaces;
using UnityEngine;

namespace Runtime.AI
{
    public class NoiseEmitter : MonoBehaviour
    {
        // ====================== Variables ======================
        
        [Header("EMITTER SETTINGS")]
        [SerializeField, Min(0), Tooltip("Noise max radius")] private float radius = 50f;

        [Header("LAYER MASK SETTINGS")]
        [SerializeField, Tooltip("Objects capable of hearing noise")] private LayerMask targetMask;

        // ====================== Properties ======================
        
        public float Radius
        {
            get => radius;
            set => radius = value;
        }
        
        // ====================== Methods ======================
        
        protected void FindTargets()
        {
            var targetsInRadius = Physics2D.OverlapCircleAll(transform.position, radius, targetMask);
            foreach (var t in targetsInRadius)
            {
                if(t.gameObject != gameObject)
                {
                    var hearing = t.GetComponentInChildren<IHearingHandler>();
                    if (hearing != null)
                    {
                        hearing.OnHearing(this);
                    }
                }
            }
        }
        
        public void EmitShot()
        {
            FindTargets();
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.15f, 0.67f, 0.88f);
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}