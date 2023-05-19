/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System.Collections;
using Runtime.AI.Interfaces;
using UnityEngine;

namespace Runtime.AI
{
    public class Sentinel : MonoBehaviour, ISightHandler
    {
        [SerializeField] private float cooldown = 3f;
        [SerializeField] private float activeTime = 8f;
        
        [Tooltip("Masks that block field of view"), SerializeField] private LayerMask obstacleMask;
        [Tooltip("Masks that contains the player character"), SerializeField] private LayerMask playerMask;
        [Tooltip("Maximum view distance"), SerializeField] private float viewRadius = 5.0f;
        [Tooltip("Maximum angle that the monster can see"), SerializeField, Range(0f, 360f)] private float viewAngle = 135.0f;
        [Tooltip("The direction to look in."), Range(0f, 360f)] public float lookAngle;
        
        public bool debug;
        public GameObject sightVisualPrefab;
        [Tooltip("Colour of the view cone when the monster is idle"), SerializeField] private Color idleColour = new(0.0f, 0.0f, 0.0f, 150.0f);
        [Tooltip("Colour of the view cone when the monster spots the player"), SerializeField] private Color aggroColour = new(255.0f, 0.0f, 0.0f, 150.0f);

        // ====================== Private Variables ======================
        private bool _isActivated;
        private Material _material;
        private bool _canSeePlayer;

        private float _cooldownTimeLeft;
        private float _activeTimeLeft;

        private void Awake()
        {
            GameObject sightVisual = Instantiate(sightVisualPrefab, transform);
            _material = sightVisual.GetComponent<MeshRenderer>().material;
            _material.color = idleColour;
        }

        private void Start()
        {
            StartCoroutine(TimeOutActivate());
        }
        
        // ====================== Interface ======================

        public bool IsActivated => _isActivated;
        public float ViewAngle => viewAngle;
        public float ViewRadius => viewRadius;
        public LayerMask ObstacleMask => obstacleMask;
        public LayerMask PlayerMask => playerMask;
        
        public void OnSightEnter()
        {
            DeactivateSentinel();
            _canSeePlayer = true;
            _material.color = aggroColour;
        }

        public void OnSightExit(Vector2 lastSeenPosition)
        {
            if (_canSeePlayer)
            {
                //event
                _material.color = idleColour;
            }
            _canSeePlayer = false;
        }

        private IEnumerator TimeOutActivate()
        {
            yield return new WaitForSeconds(4f);
            ActivateSentinel();
        }
        
        private void Update()
        {
            if (_isActivated)
            {
                if (_activeTimeLeft > 0f)
                {
                    _activeTimeLeft -= Time.deltaTime;
                    if (_activeTimeLeft <= 0f)
                    {
                        _isActivated = false;
                        _cooldownTimeLeft = cooldown;
                    }
                }
            }
            else
            {
                if (_cooldownTimeLeft > 0f)
                {
                    _cooldownTimeLeft -= Time.deltaTime;
                    if (_cooldownTimeLeft <= 0f)
                    {
                        _cooldownTimeLeft = 0f;
                    }
                }
            }
        }
        
        public void ActivateSentinel()
        {
            if(_cooldownTimeLeft > 0f) return;
            _isActivated = true;
            _activeTimeLeft = activeTime;
        }
        
        public void DeactivateSentinel()
        {
            _activeTimeLeft = 0f;
            _cooldownTimeLeft = cooldown;
            _isActivated = false;
        }
        
        public Vector2 DirFromAngle(float angleDeg)
        {
            angleDeg += lookAngle;
            return new Vector2(Mathf.Sin(angleDeg * Mathf.Deg2Rad), Mathf.Cos(angleDeg * Mathf.Deg2Rad));
        }
    }
}
