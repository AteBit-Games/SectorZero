/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System.Collections;
using Runtime.AI.Interfaces;
using UnityEngine;

namespace Runtime.AI
{
    public class NoiseEmitter : MonoBehaviour
    {
        // ====================== Variables ======================
        
        [Header("EMITTER SETTINGS")]
        [SerializeField, Min(0), Tooltip("Noise max radius")] private float radius = 50f;
        [SerializeField, Range(0, 1), Tooltip("Noise intensity")] private float intensity = 1f;
        [SerializeField, Tooltip("Is the noise being emitted")] private bool emit;
        
        [Header("LAYER MASK SETTINGS")]
        [SerializeField, Tooltip("Objects capable of hearing noise")] private LayerMask targetMask;

        [Header("GENERAL SETTINGS")]
        [SerializeField, Range(0, 1), Tooltip("Delay between updates")] private float delayBetweenUpdates = 0.2f;

        private Coroutine _findTargetCoroutine;
        private WaitForSeconds _delaySecondsUpdate;
        private readonly WaitForFixedUpdate _waitFixedUpdate = new();

        // ====================== Properties ======================
        
        public float Intensity
        {
            set => intensity = value;
        }

        public float Radius
        {
            get => radius;
            set => radius = value;
        }

        public bool IsEmitting => emit;

        // ====================== Unity Methods ======================
        
        private void Start()
        {
            _delaySecondsUpdate = new WaitForSeconds(delayBetweenUpdates);
        }
        
        private void OnEnable()
        {
            if(emit) Emit();
        }
        
        // ====================== Methods ======================
        
        private static bool IsAudible(float intensity, float lowerHearingThreshold)
        {
            return intensity >= lowerHearingThreshold;
        }
        
        public bool IsAudible(Vector3 position, float lowerHearingThreshold)
        {
            return Vector3.Distance(this.transform.position, position) <= Radius && IsAudible(intensity, lowerHearingThreshold);
        }
        
        protected void FindTargets(float noiseIntensity)
        {
            var targetsInRadius = Physics2D.OverlapCircleAll(transform.position, radius, targetMask);
            foreach (var t in targetsInRadius)
            {
                if(t.gameObject != gameObject)
                {
                    if (t.TryGetComponent<IHearingHandler>(out var hearing))
                    {
                        if (IsAudible(noiseIntensity, hearing.LowerHearingThreshold))
                        {
                            hearing.OnHearing(this, noiseIntensity);
                        }
                    }
                }
            }
        }
        
        public void EmitShot(float noiseIntensity)
        {
            FindTargets(noiseIntensity);
        }
        
        public void Emit()
        {
            emit = true;
            _findTargetCoroutine ??= StartCoroutine(FindTargetsWithDelay());
        }
        
        public void Stop()
        {
            emit = false;
            if (_findTargetCoroutine != null)
            {
                StopCoroutine(_findTargetCoroutine);
                _findTargetCoroutine = null;
            }
        }

        private IEnumerator FindTargetsWithDelay()
        {
            while (isActiveAndEnabled && emit)
            {
                yield return _waitFixedUpdate;
                FindTargets(intensity);
                yield return _delaySecondsUpdate;
            }
            _findTargetCoroutine = null;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.15f, 0.67f, 0.88f);
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}