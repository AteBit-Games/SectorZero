/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using System.Collections;
using Runtime.AI.Interfaces;
using Runtime.BehaviourTree;
using UnityEngine;
using ElRaccoone.Tweens;
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
        [Tooltip("The direction to look in.")] public Color defaultColor;
        [Tooltip("The direction to look in.")] public Color alertColor;
        
        public bool debug;
        public GameObject sightVisualPrefab;

        //---- Private Variables ----//
        private BehaviourTreeOwner _voidmask;
        private Animator _animator;
        private Light2D _light;
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
        
        //=============================== Unity Events =================================//

        private void Awake()
        {
            _voidmask = FindFirstObjectByType<BehaviourTreeOwner>(FindObjectsInactive.Include);
            _animator = GetComponent<Animator>();
            
            _light = GetComponentInChildren<Light2D>();
            _initialIntensity = _light.intensity;

            Instantiate(sightVisualPrefab, transform);
        }

        private void Start()
        {
            _light.pointLightOuterRadius = viewRadius;
            _light.pointLightInnerAngle = viewAngle;
            _light.transform.rotation = Quaternion.Euler(0f, 0f, -lookAngle-90);
            _light.color = defaultColor;
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

            _light.enabled = true;
            _light.color = defaultColor;
            _light.intensity = _initialIntensity;
            
            _animator.SetBool(Activated, true);
        }

        private void TriggerSentinel()
        {
            if (_isActivated)
            {
                _isActivated = false;
                _light.color = alertColor;
                
                if(_voidmask.treeStates.Find(x => x.state == TreeState.State.SentinelAlert) == null) Debug.LogError("No Sentinel state found");
                else _voidmask.SetActiveState(_voidmask.treeStates.Find(x => x.state == TreeState.State.SentinelAlert).stateIndex);
                
                StartCoroutine(TriggerDeactivate());
            }
        }

        private void DeactivateSentinel()
        {
            _isActivated = false;
            _activeTimeLeft = 0f;
            _light.enabled = false;
            
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
        
        //================================= Coroutine ===================================//

        private IEnumerator TriggerDeactivate()
        {
            for(var i = 0; i < 1; i++)
            {
                _light.enabled = true;
                yield return new WaitForSeconds(Random.Range(0.2f, 0.3f));
                _light.enabled = false;
                yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
            }
            
            _light.enabled = true;
            _light.TweenValueFloat(0f, 1.4f, value =>
            {
                _light.intensity = value;
            }).SetFrom(_light.intensity).SetEaseBackOut().SetOnComplete(DeactivateSentinel);
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
