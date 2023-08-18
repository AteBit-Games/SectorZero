/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using ElRaccoone.Tweens;
using ElRaccoone.Tweens.Core;
using Runtime.AI;
using Runtime.AI.Interfaces;
using Runtime.Managers;
using Runtime.Player;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Runtime.BehaviourTree 
{
    [Serializable]
    public class TreeState
    {
        public enum State
        {
            Idle,
            PatrolNear,
            PatrolFar,
            InspectSound,
            LastKnown,
            SentinelAlert,
            AggroChase,
            AggroInspect,
            Kill
        }
        
        public State state;
        public int stateIndex;
    }
    
    [AddComponentMenu("BehaviourTree/BehaviourTreeOwner")]
    public class BehaviourTreeOwner : MonoBehaviour, IHearingHandler, ISightHandler
    {
        [Tooltip("BehaviourTree asset to instantiate during Awake")] 
        public BehaviourTree behaviourTree;

        [Tooltip("Override blackboard values from the behaviour tree asset")]
        public List<BlackboardKeyValuePair> blackboardOverrides = new();
        public List<TreeState> treeStates = new();
        
        [Tooltip("Masks that block field of view"), SerializeField] private LayerMask obstacleMask;
        [Tooltip("Wall Mask"), SerializeField] private LayerMask wallMask;
        [Tooltip("Masks that contains the player character"), SerializeField] private LayerMask playerMask;
        [Tooltip("Maximum view distance"), SerializeField] private float viewRadius = 5.0f;
        [Tooltip("Maximum angle that the monster can see"), SerializeField, Range(0f, 360f)] private float viewAngle = 135.0f;
        
        [Header("SOUNDS")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<Sound> footstepSounds;
        [SerializeField] private Sound detectedSound;
        
        public bool debug;
        public GameObject sightVisualPrefab;
        [Tooltip("Colour of the view cone when the monster is idle"), SerializeField] private Color idleColour = new(0.0f, 0.0f, 0.0f, 150.0f);
        [Tooltip("Colour of the view cone when the monster spots the player"), SerializeField] private Color aggroColour = new(255.0f, 0.0f, 0.0f, 150.0f);
        
        // ====================== Private Variables ======================
        private Material _material;
        private bool _canSeePlayer;
        private bool _sightCoroutineRunning;
        private Context _context;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        
        private BlackboardKey<int> _stateReference;
        private BlackboardKey<Vector2> _lastKnownLocationReference;
        private static readonly int MoveX = Animator.StringToHash("moveX");
        private static readonly int MoveY = Animator.StringToHash("moveY");
        private static readonly int IsMoving = Animator.StringToHash("isMoving");

        private Coroutine _loseSightCoroutine;
        private Tween<float> _activeTween;

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
            
            var sightVisual = Instantiate(sightVisualPrefab, transform);
            _material = sightVisual.GetComponent<MeshRenderer>().material;
            _material.color = idleColour;
            
            _stateReference = FindBlackboardKey<int>("ActiveState");
            _lastKnownLocationReference = FindBlackboardKey<Vector2>("LastKnown");

            _navMeshAgent = _context.agent;
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            _animator = GetComponent<Animator>();
        }
        
        public void SetActiveState(int state)
        {
            _stateReference.value = state;
        }

        private void Update() 
        {
            if (behaviourTree) behaviourTree.Update();
            _animator.transform.position = _navMeshAgent.transform.position;
            _animator.SetBool(IsMoving, _navMeshAgent.velocity.magnitude > 0.1f);
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
        public LayerMask WallMask => wallMask;
        public LayerMask PlayerMask => playerMask;
        [HideInInspector] public bool isPlayerCrouching;
        
        
        public void OnHearing(NoiseEmitter sender)
        {
            if (!_canSeePlayer)
            {
                if(treeStates.Find(x => x.state == TreeState.State.InspectSound) == null) Debug.LogError("No Inspect state found");
                else
                {
                    _lastKnownLocationReference.value = sender.transform.position;
                    SetActiveState(treeStates.Find(x => x.state == TreeState.State.InspectSound).stateIndex);
                }
            }
        }
        
        public void OnSightEnter()
        {
            if (_sightCoroutineRunning && _loseSightCoroutine != null)
            {
                StopCoroutine(_loseSightCoroutine);
                _sightCoroutineRunning = false;
            }
            
            if (!_canSeePlayer)
            {
                
                var volume = FindFirstObjectByType<Volume>();
                var vignette = volume.sharedProfile.components[0] as Vignette;

                if(_activeTween != null) _activeTween.Cancel();
                _activeTween = volume.TweenValueFloat(0.3f, 1.4f, value =>
                {
                    if (vignette != null) vignette.intensity.value = value;
                }).SetFrom(0f).SetEaseSineInOut();
                
                GameManager.Instance.SoundSystem.PlaySting(detectedSound);
                
                if(treeStates.Find(x => x.state == TreeState.State.AggroChase) == null) Debug.LogError("No aggro state found");
                else
                {
                    _canSeePlayer = true;
                    _material.color = aggroColour;
                    SetActiveState(treeStates.Find(x => x.state == TreeState.State.AggroChase).stateIndex);
                }
                
                _sightCoroutineRunning = false;
            }
        }

        public void OnSightExit(Vector2 lastKnownPosition)
        {
            if(_canSeePlayer && !_sightCoroutineRunning)
            {
                _lastKnownLocationReference.value = lastKnownPosition;
                _loseSightCoroutine = StartCoroutine(LoseSight());
                _sightCoroutineRunning = true;
            }
        }
        
        private IEnumerator LoseSight()
        {
            yield return new WaitForSeconds(1f);
            
            FindFirstObjectByType<PlayerController>().GetComponent<ISightEntity>().IsSeen = false;
            _material.color = idleColour;
            
            var volume = FindFirstObjectByType<Volume>();
            var vignette = volume.sharedProfile.components[0] as Vignette;
            
            if(_activeTween != null) _activeTween.Cancel();
            _activeTween = volume.TweenValueFloat(0f, 1f, value =>
            {
                if (vignette != null) vignette.intensity.value = value;
            }).SetFrom(vignette.intensity.value).SetEaseSineInOut();
            
            if(treeStates.Find(x => x.state == TreeState.State.LastKnown) == null) Debug.LogError("No last known state specified");
            else
            {
                _material.color = idleColour;
                SetActiveState(treeStates.Find(x => x.state == TreeState.State.LastKnown).stateIndex);
            }
            
            _loseSightCoroutine = null;
            _sightCoroutineRunning = false;
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
            audioSource.volume = sound.volumeScale;
            audioSource.PlayOneShot(sound.clip);
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
        
        public void SetLookDirection(Vector2 direction)
        {
            _animator.SetFloat(MoveX, direction.x);
            _animator.SetFloat(MoveY, direction.y);
        } 
    }
}