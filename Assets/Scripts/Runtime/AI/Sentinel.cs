/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.AI.Interfaces;
using UnityEngine;
using ElRaccoone.Tweens;
using Runtime.Managers;
using Runtime.Misc.Triggers;
using Runtime.SoundSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Runtime.AI
{
    public class Sentinel : MonoBehaviour, ISightHandler
    {
        [Tooltip("Masks that block field of view"), SerializeField] private LayerMask obstacleMask;
        [Tooltip("Wall Mask"), SerializeField] private LayerMask wallMask;
        [Tooltip("Masks that contains the player character"), SerializeField] private LayerMask playerMask;
        [Tooltip("Maximum view distance"), SerializeField] private float viewRadius = 5.0f;
        [Tooltip("Maximum angle that the monster can see"), SerializeField, Range(0f, 360f)] private float viewAngle = 135.0f;
        [Tooltip("The direction to look in."), Range(0f, 360f)] public float lookAngle;
        
        public float deactivateDelay;
        public bool startActive;

        public Sound detectSound;
        
        public float intensity;
        public Color defaultColor;
        public Color alertColor;
        
        public bool debug;
        public GameObject sightVisualPrefab;
        
        private Animator _animator;
        private List<Light2D> _eyeLights;
        private float _initialIntensity;

        private bool _isActivated;
        private float _activeTimeLeft;
        private static readonly int Activated = Animator.StringToHash("activated");

        // ====================== Interface ======================

        public bool IsActivated => _isActivated;
        public float ViewAngle => viewAngle;
        public float ViewRadius => viewRadius;
        public LayerMask ObstacleMask => obstacleMask;
        public LayerMask PlayerMask => playerMask;
        public LayerMask WallMask => wallMask;
        
        public Action OnSightEnterAction { get; set; }
        
        //=============================== Unity Events =================================//

        private void Awake()
        {
            var parent = transform.parent;
            _animator = parent.GetComponentInChildren<Animator>();
            parent.GetComponentInChildren<TriggerDelegate>().triggerEvent += ActivateLights;
            
            _eyeLights = GetComponentsInChildren<Light2D>().ToList();
            _eyeLights.ForEach(eye =>
            {
                eye.enabled = false;
                eye.intensity = intensity;
            });
            
            
            Instantiate(sightVisualPrefab, transform);
        }

        private void Start()
        {
            _eyeLights.ForEach(eye =>
            {
                eye.color = defaultColor;
                eye.intensity = intensity;
            });
            
            if(startActive) ActivateSentinel(999f);
        }
        
        private void Update()
        {
            if (_isActivated && _activeTimeLeft > 0f)
            {
                _activeTimeLeft -= Time.deltaTime;
                if (_activeTimeLeft <= 0f)
                {
                    _isActivated = false;
                    DeactivateSentinel();
                }
            }
        }
        
        //========================== Public Methods ==============================//
        
        public void OnSightEnter()
        {
            TriggerSentinel();
        }
        
        public void OnSightExit(Vector2 lastSeenPosition)
        {
            //Not Used
        }
        
        public void ActivateSentinel(float duration)
        {
            _isActivated = true;
            _activeTimeLeft = duration;
            
            _eyeLights.ForEach(eye =>
            {
                eye.color = defaultColor;
                eye.intensity = _initialIntensity;
            });
            
            _animator.SetBool(Activated, true);
        }

        private void TriggerSentinel()
        {
            if (_isActivated)
            {
                _isActivated = false;
                _eyeLights.ForEach(eye =>
                {
                    eye.color = alertColor;
                });
                
                var hit = Physics2D.OverlapPoint(transform.position, 1 << LayerMask.NameToLayer("RoomBounds"));
                if(hit == null) Debug.LogWarning("No RoomBounds found for " + gameObject.name);
                OnSightEnterAction?.Invoke();
                
                StartCoroutine(TriggerEyes());
                StartCoroutine(TriggerDeactivate());
                StartCoroutine(TriggerDetection());
            }
        }

        private void DeactivateSentinel()
        {
            _isActivated = false;
            _activeTimeLeft = 0f;
            
            _animator.SetBool(Activated, false);
        }
        
        //================================= Helpers ===================================//
        
        public Vector2 DirFromAngle(float angleDeg)
        {
            angleDeg += lookAngle;
            return new Vector2(Mathf.Sin(angleDeg * Mathf.Deg2Rad), Mathf.Cos(angleDeg * Mathf.Deg2Rad));
        }
        
        private Vector2 GetAngleFromLook()
        {
            return new Vector2(Mathf.Sin((lookAngle+90) * Mathf.Deg2Rad), Mathf.Cos((lookAngle+90) * Mathf.Deg2Rad));
        }

        private void ActivateLights()
        {
            _eyeLights.ForEach(eye =>
            {
                eye.enabled = true;
                eye.intensity = intensity;
            });
        }
        
        //================================= Coroutine ===================================//

        private IEnumerator TriggerDetection()
        {
            GameManager.Instance.SoundSystem.PlaySting(detectSound);
            
            var volume = FindFirstObjectByType<Volume>();
            var vignette = volume.sharedProfile.components[0] as Vignette;
            
            volume.TweenValueFloat(0.2f, 1.4f, value =>
            {
                if (vignette != null) vignette.intensity.value = value;
            }).SetFrom(0f).SetEaseSineInOut();
            
            yield return new WaitForSeconds(2f);
            
            volume.TweenValueFloat(0, 1f, value =>
            {
                if (vignette != null) vignette.intensity.value = value;
            }).SetFrom(0.2f).SetEaseSineInOut();
        }
        
        private IEnumerator TriggerDeactivate()
        {
            yield return new WaitForSeconds(deactivateDelay);
            DeactivateSentinel();
        }

        private IEnumerator TriggerEyes()
        {
            //_eyeLights.ForEach(eye => eye.enabled = true);
            _eyeLights.ForEach(eye => eye.intensity = intensity);
            yield return new WaitForSeconds(Random.Range(0.2f, 0.3f));
                
            // _eyeLights.ForEach(eye => eye.enabled = false);
            _eyeLights.ForEach(eye => eye.intensity = intensity + 0.2f);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
            
            _eyeLights.ForEach(eye =>
            {
                //eye.enabled = true;
                eye.TweenValueFloat(0f, 1.4f, value =>
                {
                    eye.intensity = value;
                }).SetFrom(eye.intensity).SetEaseBackOut();
            });
        }

        private void OnDrawGizmosSelected()
        {
            if (!debug) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, viewRadius);
            
            //Debug.DrawRay(transform.position, GetAngleFromLook()*viewRadius, Color.red);

            //from th middle line get the left and right line
            var leftLine = GetAngleFromLook() * viewRadius;
            var rightLine = GetAngleFromLook() * viewRadius;
            
            //rotate the left and right line by the view angle
            leftLine = Quaternion.AngleAxis(-viewAngle / 2, Vector3.forward) * leftLine;
            rightLine = Quaternion.AngleAxis(viewAngle / 2, Vector3.forward) * rightLine;
            
            //draw the left and right line
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, leftLine);
            Gizmos.DrawRay(transform.position, rightLine);
        }
    }
}
