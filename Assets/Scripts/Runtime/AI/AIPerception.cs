/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using System.Collections;
using ElRaccoone.Tweens;
using ElRaccoone.Tweens.Core;
using Runtime.AI.Interfaces;
using Runtime.BehaviourTree.Monsters;
using Runtime.Managers;
using Runtime.Player;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Runtime.AI
{
    [DefaultExecutionOrder(10)]
    public class AIPerception : MonoBehaviour, IHearingHandler, ISightHandler
    {
        [SerializeField] private BehaviourTreeOwner treeOwner;
        
        [Tooltip("Masks that block field of view"), SerializeField] private LayerMask obstacleMask;
        [Tooltip("Wall Mask"), SerializeField] private LayerMask wallMask;
        [Tooltip("Masks that contains the player character"), SerializeField] private LayerMask playerMask;
        [SerializeField] private PlayerController player;
        
        [Tooltip("Maximum view distance"), SerializeField] private float viewRadius = 5.0f;
        [Tooltip("Maximum angle that the monster can see"), SerializeField, Range(0f, 360f)] private float viewAngle = 135.0f;
        [Tooltip("Radius to instantly see player"), SerializeField] private float detectRadius = 5.0f;
        
        [SerializeField] private Sound detectedSound;
        [SerializeField] private Sound lostSound;
        
        private Volume _volume;
        public bool debug;
        
        //----- Interfaces -----//
        public Action OnSightEnterAction { get; set; }

        public LayerMask PlayerMask => playerMask;
        public LayerMask WallMask => wallMask;
        
        // ====================== Private Variables ======================
        private bool _canSeePlayer;
        private bool _looseSightCoroutineRunning;
        private bool _gainSightCoroutineRunning;

        private Coroutine _loseSightCoroutine;
        private Coroutine _gainSightCoroutine;
        private Tween<float> _activeVignetteTween;
        private Tween<float> _activeAberrationTween;

        // ====================== Unity Events ======================

        private void Start()
        {
            _volume = FindFirstObjectByType<Volume>();
            StartCoroutine(SightRoutine());
        }

        // ====================== Interface Methods ======================
        
        public void OnHearing(NoiseEmitter sender)
        {
            if (!_canSeePlayer)
            {
                switch (treeOwner)
                {
                    case Vincent vincent:
                    {
                        Debug.Log("Heard");
                        vincent.SetHeard(sender.transform.position);
                        break;
                    }
                    case VoidMask voidMask:
                    {
                        if(!voidMask.StateOverride())
                        {
                            voidMask.SetHeard(sender.transform.position);
                        }
                        break;
                    }
                }
            }
        }

        public void OnSightEnter()
        {
            if (_looseSightCoroutineRunning && _loseSightCoroutine != null)
            {
                StopCoroutine(_loseSightCoroutine);
                _loseSightCoroutine = null;
                _looseSightCoroutineRunning = false;
            }

            if (!_canSeePlayer && !_gainSightCoroutineRunning)
            {
                _gainSightCoroutine = StartCoroutine(GainSight(false));
                _gainSightCoroutineRunning = true;
            }
        }
        
        private IEnumerator GainSight(bool instant)
        {
            if (_volume.sharedProfile.components[0] is Vignette vignette)
            {
                if (_activeVignetteTween != null) _activeVignetteTween.Cancel();
                _activeVignetteTween = _volume.TweenValueFloat(0.3f, 1.4f, value =>
                {
                    if (vignette != null) vignette.intensity.value = value;
                }).SetFrom(vignette.intensity.value).SetEaseSineInOut();
            }

            if(!instant) yield return new WaitForSeconds(1.2f);
            else yield return null;

            if (_volume.sharedProfile.components[1] is ChromaticAberration aberration)
            {
                if (_activeAberrationTween != null) _activeAberrationTween.Cancel();
                _activeAberrationTween = _volume.TweenValueFloat(0.8f, 1f, value =>
                {
                    if (aberration != null) aberration.intensity.value = value;
                }).SetFrom(0).SetEaseSineInOut();
            }

            GameManager.Instance.SoundSystem.PlaySting(detectedSound);
            treeOwner.SetState(MonsterState.AggroChase);
            _canSeePlayer = true;
            player.GetComponent<ISightEntity>().IsSeen = false;
            
            OnSightEnterAction?.Invoke();
            _gainSightCoroutine = null;
            _gainSightCoroutineRunning = false;
        }

        public void OnSightExit(Vector2 lastKnownPosition)
        {
            if(_gainSightCoroutineRunning && _gainSightCoroutine != null)
            {
                StopCoroutine(_gainSightCoroutine);
                _gainSightCoroutine = null;
                _gainSightCoroutineRunning = false;

                if (_volume.sharedProfile.components[0] is Vignette vignette)
                {
                    if (_activeVignetteTween != null) _activeVignetteTween.Cancel();
                    _activeVignetteTween = _volume.TweenValueFloat(0f, 1f, value =>
                    {
                        if (vignette != null) vignette.intensity.value = value;
                    }).SetFrom(vignette.intensity.value).SetEaseSineInOut();
                }

                return;
            }
            
            var didSeePlayerEnterHidable = treeOwner.DidSeeEnter();
            if(_canSeePlayer && !_looseSightCoroutineRunning && !didSeePlayerEnterHidable)
            {
                treeOwner.SetLastKnownPosition(lastKnownPosition);
                _loseSightCoroutine = StartCoroutine(LoseSight());
                _looseSightCoroutineRunning = true;
            }
        }
        
        private IEnumerator LoseSight()
        {
            yield return new WaitForSeconds(2f);
            
            GameManager.Instance.SoundSystem.PlaySting(lostSound);
            player.GetComponent<ISightEntity>().IsSeen = false;

            if (_volume.sharedProfile.components[0] is Vignette vignette)
            {
                if(_activeVignetteTween != null) _activeVignetteTween.Cancel();
                _activeVignetteTween = _volume.TweenValueFloat(0f, 1f, value =>
                {
                    if (vignette != null) vignette.intensity.value = value;
                }).SetFrom(vignette.intensity.value).SetEaseSineInOut();
            }
            
            if(_volume.sharedProfile.components[1] is ChromaticAberration aberration)
            {
                if(_activeAberrationTween != null) _activeAberrationTween.Cancel();
                _activeAberrationTween = _volume.TweenValueFloat(0f, 1f, value =>
                {
                    if (aberration != null) aberration.intensity.value = value;
                }).SetFrom(aberration.intensity.value).SetEaseSineInOut();
            }

            treeOwner.SetState(MonsterState.Patrol);
            _loseSightCoroutine = null;
            _looseSightCoroutineRunning = false;
            _canSeePlayer = false;
            
            yield return null;
        }
        
        // ====================== Helper Methods ======================
        
        private IEnumerator SightRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                FieldOfViewCheck();
            }
        }

        private void FieldOfViewCheck()
        {
            if(player.isHiding) return;
            var rangeChecks = Physics2D.OverlapCircleAll(transform.position, viewRadius, playerMask);
            
            if (rangeChecks.Length != 0)
            {
                var target = rangeChecks[0].transform;
                var position = transform.position;
                var targetPosition = target.position;
                
                var distanceToTarget = Vector3.Distance(position, targetPosition);
                
                //Determine the cone radius and direction to check
                var directionToTarget = (targetPosition - position).normalized;
                var direction = DirectionFromAngleCenter(transform.eulerAngles.z);
                var angle = Vector2.Angle(direction, directionToTarget);
                
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
                            if (!Physics2D.Raycast(position, directionToTarget, distanceToTarget, obstacleMask))
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
                    else OnSightExit(player.transform.position);
                }
                else
                {
                    if(distanceToTarget < detectRadius)
                    {
                        if (!Physics2D.Raycast(position, directionToTarget, distanceToTarget, obstacleMask))
                        {
                            if(!_canSeePlayer)
                            {
                                StartCoroutine(GainSight(true));
                            }
                        }
                    }
                    else OnSightExit(player.transform.position);
                }
            }
            else OnSightExit(player.transform.position);
        }
        
        // ====================== Private Methods ======================
        
        private static Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
        {
            angleInDegrees += eulerY;
            return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0);
        }
        
        private static Vector2 DirectionFromAngleCenter(float angleInDegrees)
        {
            return new Vector2(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad));
        }
        
        // =========================== Debug ============================
        
        private void OnDrawGizmos()
        {
            if(!debug) return;
            
            var position = transform.position;
            
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(position, viewRadius);

            var eulerAngles = transform.eulerAngles;
            var viewAngle01 = DirectionFromAngle(eulerAngles.z, -viewAngle / 2);
            var viewAngle02 = DirectionFromAngle(eulerAngles.z, viewAngle / 2);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(position, position + viewAngle01 * viewRadius);
            Gizmos.DrawLine(position, position + viewAngle02 * viewRadius);
            
            if (_canSeePlayer)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, FindFirstObjectByType<PlayerController>().transform.position);
            }
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, detectRadius);
        }
    }
}
