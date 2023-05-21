/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System.Collections.Generic;
using Runtime.AI;
using Runtime.AI.Interfaces;
using Runtime.Player;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Runtime.BehaviourTree 
{
    [AddComponentMenu("BehaviourTree/BehaviourTreeOwner")]
    public class BehaviourTreeOwner : MonoBehaviour, IHearingHandler, ISightHandler
    {
        [Tooltip("BehaviourTree asset to instantiate during Awake")] 
        public BehaviourTree behaviourTree;
        [Tooltip("Run behaviour tree validation at startup")] 
        public bool validate = true;
        
        [Tooltip("Override blackboard values from the behaviour tree asset")]
        public List<BlackboardKeyValuePair> blackboardOverrides = new();
        
        [Tooltip("Masks that block field of view"), SerializeField] private LayerMask obstacleMask;
        [Tooltip("Masks that contains the player character"), SerializeField] private LayerMask playerMask;
        [Tooltip("Maximum view distance"), SerializeField] private float viewRadius = 5.0f;
        [Tooltip("Maximum angle that the monster can see"), SerializeField, Range(0f, 360f)] private float viewAngle = 135.0f;
        
        [Header("SOUNDS")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<AudioClip> footstepSounds;
        
        public bool debug;
        public GameObject sightVisualPrefab;
        [Tooltip("Colour of the view cone when the monster is idle"), SerializeField] private Color idleColour = new(0.0f, 0.0f, 0.0f, 150.0f);
        [Tooltip("Colour of the view cone when the monster spots the player"), SerializeField] private Color aggroColour = new(255.0f, 0.0f, 0.0f, 150.0f);
        
        // ====================== Private Variables ======================
        private Material _material;
        private bool _canSeePlayer;
        private Context _context;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        
        private BlackboardKey<int> _stateReference;
        private BlackboardKey<Vector2> _investigateLocationReference;

        // ====================== Unity Events ======================
        
        private void Awake() 
        {
            bool isValid = ValidateTree();
            if (isValid) 
            {
                _context = CreateBehaviourTreeContext();
                behaviourTree = behaviourTree.Clone();
                behaviourTree.Bind(_context);
                ApplyKeyOverrides();
            }
            else
            {
                behaviourTree = null;
            }
            
            GameObject sightVisual = Instantiate(sightVisualPrefab, transform);
            _material = sightVisual.GetComponent<MeshRenderer>().material;
            _material.color = idleColour;
            
            _stateReference = FindBlackboardKey<int>("ActiveState");
            _investigateLocationReference = FindBlackboardKey<Vector2>("InvestigateLocation");
            
            _navMeshAgent = _context.agent;
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            _animator = GetComponent<Animator>();
        }

        private void Update() 
        {
            if (behaviourTree) behaviourTree.Update();
            _animator.transform.position = _navMeshAgent.transform.position;
            
            Vector3 movement = _navMeshAgent.velocity;
            if (movement.magnitude > 0.1f)
            {
                _animator.SetFloat("moveX", movement.x);
                _animator.SetFloat("moveY", movement.z);
            }
            _animator.SetBool("isMoving", movement.magnitude > 0.1f);
        }

        private void OnDrawGizmos() 
        {
            if (!behaviourTree || !Application.isPlaying) return;

            BehaviourTree.Traverse(behaviourTree.rootNode, node => {
                if(node.drawGizmos) node.OnDrawGizmos();
            });
        }

        // ====================== Interface ======================

        public float ViewAngle => viewAngle;
        public float ViewRadius => viewRadius;
        public LayerMask ObstacleMask => obstacleMask;
        public LayerMask PlayerMask => playerMask;
        
        public void OnHearing(NoiseEmitter sender)
        {
            if(_stateReference.value is 2 or 3) return;
            _investigateLocationReference.value = sender.transform.position;
            _stateReference.value = 1;
        }
        
        public void OnSightEnter()
        {
            _canSeePlayer = true;
            _material.color = aggroColour;
            _stateReference.value = 2;
        }

        public void OnSightExit(Vector2 lastKnownPosition)
        {
            if (_canSeePlayer)
            {
                _investigateLocationReference.value = lastKnownPosition;
                _stateReference.value = 1;
                FindObjectOfType<PlayerController>().GetComponent<ISightEntity>().IsSeen = false;
                _material.color = idleColour;
            }
            _canSeePlayer = false;
        }
        
        // ====================== Public Methods ======================
        
        public BlackboardKey<T> FindBlackboardKey<T>(string keyName)
        {
            return behaviourTree ? behaviourTree.blackboard.Find<T>(keyName) : null;
        }

        public void SetBlackboardValue<T>(string keyName, T value)
        {
            if (behaviourTree) behaviourTree.blackboard.SetValue(keyName, value);
        }

        public T GetBlackboardValue<T>(string keyName)
        {
            return behaviourTree ? behaviourTree.blackboard.GetValue<T>(keyName) : default;
        }
        
        public Vector2 DirFromAngle(float angleDeg)
        {
            angleDeg += _context.agent.transform.eulerAngles.z;
            return new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad));
        }
        
        public void PlayFootstepSound()
        {
            var sound = footstepSounds[Random.Range(0, footstepSounds.Count)];
            audioSource.PlayOneShot(sound);
        }

        // ====================== Private Methods ======================
        
        private void ApplyKeyOverrides() 
        {
            foreach(var pair in blackboardOverrides)
            {
                var targetKey = behaviourTree.blackboard.Find(pair.key.name);
                var sourceKey = pair.value;
                if (targetKey != null && sourceKey != null) targetKey.CopyFrom(sourceKey);
            }
        }
        
        private Context CreateBehaviourTreeContext() 
        {
            return Context.CreateFromGameObject(gameObject);
        }

        private bool ValidateTree() 
        {
            if (!behaviourTree) 
            {
                Debug.LogWarning($"No BehaviourTree assigned to {name}, assign a behaviour tree in the inspector");
                return false;
            }

            return true;
        }
    }
}