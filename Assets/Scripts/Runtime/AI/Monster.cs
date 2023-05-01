using System.Collections.Generic;
using Runtime.AI.Interfaces;
using Runtime.Navigation;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.AI
{
    public class Monster : MonoBehaviour, IHearingHandler, ISightHandler
    {
        // ====================== Variables ======================
        [Header("SIGHT SETTINGS")]
        [Tooltip("Masks that block field of view"), SerializeField] private LayerMask obstacleMask;
        [Tooltip("Masks that contains the player character"), SerializeField] private LayerMask playerMask;
        [Tooltip("Maximum view distance"), SerializeField] private float viewRadius = 5.0f;
        [Tooltip("Maximum angle that the monster can see"), SerializeField] private float viewAngle = 135.0f;
        
        [Header("Hearing")]
        [Range(0, 1), Tooltip("Minimum noise intensity able to hear"), SerializeField] private float hearingThreshold = 0.3f;
        
        [Header("DEBUG")]
        public bool debug;
        public GameObject sightVisualPrefab;
        [Tooltip("Colour of the view cone when the monster is idle"), SerializeField] private Color idleColor = new(0.0f, 0.0f, 0.0f, 150.0f);
        [Tooltip("Colour of the view cone when the monster spots the player"), SerializeField] private Color aggroColour = new(255.0f, 0.0f, 0.0f, 150.0f);
        
        private Material _material;
        private bool _canSeePlayer;

        private readonly List<NoiseEmitter> _noiseEmitters = new();
        private Transform _target;
        
        private NavAgent _navAgent;

        // ====================== Interface ======================

        public float ViewAngle => viewAngle;
        public float ViewRadius => viewRadius;
        public LayerMask ObstacleMask => obstacleMask;
        public LayerMask PlayerMask => playerMask;
        
        public float LowerHearingThreshold
        {
            get => hearingThreshold;
            set => hearingThreshold = value;
        }

        public void OnHearing(NoiseEmitter sender, float intensity)
        {
            bool isAlreadyInList = _noiseEmitters.Contains(sender);
            if (!isAlreadyInList)
            {
                _noiseEmitters.Add(sender);
                _navAgent.SetDestination(sender.transform.position);
            }
        }
        
        public void OnSightEnter()
        {
            _canSeePlayer = true;
            _material.color = aggroColour;
        }

        public void OnSightExit()
        {
            if (_canSeePlayer)
            {
                //event
                _material.color = idleColor;
            }
            _canSeePlayer = false;
        }

        // ====================== Unity Events ======================

        private void Update()
        {
            _noiseEmitters.RemoveAll(a => (!a.IsEmitting || Vector3.Distance(a.transform.position, transform.position) > a.Radius));
        }
        
        private void Awake()
        {
            GameObject sightVisual = Instantiate(sightVisualPrefab, transform.parent);
            _material = sightVisual.GetComponent<MeshRenderer>().material;
            _material.color = idleColor;
            _navAgent = GetComponent<NavAgent>();
        }
        
        public Vector2 DirFromAngle(float angleDeg)
        {
            angleDeg += transform.eulerAngles.z;
            return new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad));
        }
    }
}