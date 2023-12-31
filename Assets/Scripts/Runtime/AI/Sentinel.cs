/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Runtime.Managers;
using Runtime.Misc.Triggers;
using Runtime.SoundSystem;
using Tweens;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Runtime.AI
{
    public class Sentinel : MonoBehaviour
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
        
        private Animator _animator;
        private List<Light2D> _eyeLights;
        private float _initialIntensity;

        private bool _isActivated;
        private float _activeTimeLeft;
        private Coroutine _sightRoutine;

        private static readonly int Activated = Animator.StringToHash("activated");

        // ====================== Interface ======================
        
        public Action<Collider2D> OnSightEnterAction { get; set; }
        
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
                    _eyeLights.ForEach(eye =>
                    {
                        eye.color = defaultColor;
                        eye.intensity = _initialIntensity;
                    });
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
            _sightRoutine = StartCoroutine(SightRoutine());
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
                else OnSightEnterAction?.Invoke(hit);
                
                StartCoroutine(TriggerEyes());
                StartCoroutine(TriggerDeactivate());
                StartCoroutine(TriggerDetection());
            }
        }

        private void DeactivateSentinel()
        {
            _isActivated = false;
            _activeTimeLeft = 0f;
            StopCoroutine(_sightRoutine);
            _sightRoutine = null;
            
            _animator.SetBool(Activated, false);
        }
        
        //================================= Helpers ===================================//
        
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
        
        private void FieldOfViewCheck()
        {
            var rangeChecks = Physics2D.OverlapCircleAll(transform.position, viewRadius, playerMask);
            
            if (rangeChecks.Length != 0)
            {
                var target = rangeChecks[0].transform;
                var position = transform.position;
                var targetPosition = target.position;
                
                var distanceToTarget = Vector3.Distance(position, targetPosition);
                
                var directionToTarget = (targetPosition - position).normalized;
                var angle = Vector2.Angle(directionToTarget, GetAngleFromLook());
                
                //Check if the player is within the field of view
                if (angle < viewAngle / 2)
                {
                    //Check if the player is not behind a wall
                    if (!Physics2D.Raycast(position, directionToTarget, distanceToTarget, wallMask))
                    {
                        //Check if the player is crouching
                        if (GameManager.Instance.AIManager.isPlayerCrouching)
                        {
                            //Check if the player is behind an obstacle only if the player is crouching
                            if(!Physics2D.Raycast(position, directionToTarget, distanceToTarget,obstacleMask))
                            {
                                OnSightEnter();
                            }
                        }
                        else
                        {
                            //If here then player is within view and out in the open
                            OnSightEnter();
                        }
                    }
                }
            }
        }
        
        //================================= Coroutine ===================================//

        private IEnumerator SightRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                FieldOfViewCheck();
            }
        }
        
        private IEnumerator TriggerDetection()
        {
            GameManager.Instance.SoundSystem.PlaySting(detectSound);
            
            var volume = FindFirstObjectByType<Volume>();
            var vignette = volume.sharedProfile.components[0] as Vignette;
            
            var tween = new FloatTween()
            {
                easeType = EaseType.SineInOut,
                to = 0.2f,
                duration = 1.4f,
                from = 0f,
                onUpdate = (_, value) =>
                {
                    if (vignette != null) vignette.intensity.value = value;
                }
            };
            
            volume.gameObject.AddTween(tween);
            yield return new WaitForSeconds(3.4f);
            
            tween = new FloatTween()
            {
                easeType = EaseType.SineInOut,
                to = 0f,
                duration = 1f,
                from = 0.2f,
                onUpdate = (_, value) =>
                {
                    if (vignette != null) vignette.intensity.value = value;
                }
            };
            
            volume.gameObject.AddTween(tween);
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
                eye.enabled = true;
                
                
                
                // eye.TweenValueFloat(0f, 1.4f, value =>
                // {
                //     eye.intensity = value;
                // }).SetFrom(eye.intensity).SetEaseBackOut();
                
                var tween = new FloatTween()
                {
                    easeType = EaseType.SineInOut,
                    to = 0f,
                    duration = 1.4f,
                    from = eye.intensity,
                    onUpdate = (_, value) =>
                    {
                        eye.intensity = value;
                    }
                };
                
                eye.gameObject.AddTween(tween);
            });
        }

        private void OnDrawGizmosSelected()
        {
            if (!debug) return;
            
            Gizmos.color = Color.red;
            var position = transform.position;
            Gizmos.DrawWireSphere(position, viewRadius);

            //from th middle line get the left and right line
            var leftLine = GetAngleFromLook() * viewRadius;
            var rightLine = GetAngleFromLook() * viewRadius;
            
            //rotate the left and right line by the view angle, get from look angle
            leftLine = Quaternion.AngleAxis(-viewAngle / 2, Vector3.forward) * leftLine;
            rightLine = Quaternion.AngleAxis(viewAngle / 2, Vector3.forward) * rightLine;
            
            //draw the left and right line
            Gizmos.color = Color.green;
            Gizmos.DrawRay(position, leftLine);
            Gizmos.DrawRay(position, rightLine);
        }
    }
}
