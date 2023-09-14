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
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Runtime.BehaviourTree 
{
    public enum State
    {
        Idle,
        Patrol,
        InspectPoint,
        AggroInspect,
        AggroChase,
        SentinelAlert,
    }
    
    [Serializable]
    public class TreeState
    {
        public State state;
        public int stateIndex;
    }
    
    public enum Monster
    {
        Vincent,
        VoidMask,
    }
    
    [AddComponentMenu("BehaviourTree/BehaviourTreeOwner")]
    [DefaultExecutionOrder(10)]
    public class BehaviourTreeOwner : MonoBehaviour, IHearingHandler, ISightHandler, IPersistant
    {
        [Tooltip("BehaviourTree asset to instantiate during Awake")] 
        public BehaviourTree behaviourTree;
        public Monster monster;

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
        [SerializeField] private Sound lostSound;
        
        public bool debug;
        public GameObject sightVisualPrefab;
        [Tooltip("Colour of the view cone when the monster is idle"), SerializeField] private Color idleColour = new(0.0f, 0.0f, 0.0f, 150.0f);
        [Tooltip("Colour of the view cone when the monster spots the player"), SerializeField] private Color aggroColour = new(255.0f, 0.0f, 0.0f, 150.0f);
        
        //----- Interfaces -----//
        public Action OnSightEnterAction { get; set; }
        
        // ====================== Private Variables ======================
        private Material _material;
        private Context _context;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        
        private State _currentState = State.Patrol;
        private bool _canSeePlayer;
        private bool _looseSightCoroutineRunning;
        private bool _gainSightCoroutineRunning;

        private BlackboardKey<int> _stateReference;
        private BlackboardKey<Vector2> _inspectLocationReference;
        private static readonly int MoveX = Animator.StringToHash("moveX");
        private static readonly int MoveY = Animator.StringToHash("moveY");
        private static readonly int IsMoving = Animator.StringToHash("isMoving");

        private Coroutine _loseSightCoroutine;
        private Coroutine _gainSightCoroutine;
        private Tween<float> _activeVignetteTween;
        private Tween<float> _activeAberrationTween;

        // ====================== Unity Events ======================
        
        private void Awake() 
        {
            bool isValid = ValidateTree();
            if (isValid) 
            {
                behaviourTree = behaviourTree.Clone();
                
                _context = CreateBehaviourTreeContext(behaviourTree);
                behaviourTree.Bind(_context);
                ApplyKeyOverrides();
            }
            else
            {
                behaviourTree = null;
            }
            
            SetupReferences();
            GameManager.Instance.SoundSystem.SetupSound(audioSource, footstepSounds[0]);
            
            var sightVisual = Instantiate(sightVisualPrefab, transform);
            _material = sightVisual.GetComponent<MeshRenderer>().material;
            _material.color = idleColour;
            
            _navMeshAgent = _context.agent;
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            _animator = GetComponent<Animator>();
        }
        
        public void SetActiveState(int state)
        {
            _stateReference.value = state;
        }
        
        private void Start()
        {
            behaviourTree.treeState = Node.State.Running;
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
            if (!_canSeePlayer && !StateOverride())
            {
                _inspectLocationReference.value = sender.transform.position;
                SetState(State.InspectPoint);
            }
        }

        public void SetState(State state)
        {
            if(treeStates.Find(x => x.state == state) == null) Debug.LogError( state + " state not found");
            else
            {
                _currentState = state;
                SetActiveState(treeStates.Find(x => x.state == state).stateIndex);
            }
        }
        
        // ====================== Sight Functions ======================
        
        public void OnSightEnter()
        {
            if (_looseSightCoroutineRunning && _loseSightCoroutine != null)
            {
                StopCoroutine(_loseSightCoroutine);
                _looseSightCoroutineRunning = false;
            }
            
            if (!_canSeePlayer && !_gainSightCoroutineRunning )
            {
                _gainSightCoroutine = StartCoroutine(GainSight());
                _gainSightCoroutineRunning = true;
                
                var volume = FindFirstObjectByType<Volume>();
                var vignette = volume.sharedProfile.components[0] as Vignette;

                if(_activeVignetteTween != null) _activeVignetteTween.Cancel();
                _activeVignetteTween = volume.TweenValueFloat(0.3f, 1.4f, value =>
                {
                    if (vignette != null) vignette.intensity.value = value;
                }).SetFrom(0f).SetEaseSineInOut();
            }
        }

        public void OnSightExit(Vector2 lastKnownPosition)
        {
            if(_gainSightCoroutineRunning && _gainSightCoroutine != null)
            {
                StopCoroutine(_gainSightCoroutine);
                _gainSightCoroutineRunning = false;
                
                var volume = FindFirstObjectByType<Volume>();
                var vignette = volume.sharedProfile.components[0] as Vignette;
            
                if(_activeVignetteTween != null) _activeVignetteTween.Cancel();
                _activeVignetteTween = volume.TweenValueFloat(0f, 1f, value =>
                {
                    if (vignette != null) vignette.intensity.value = value;
                }).SetFrom(vignette.intensity.value).SetEaseSineInOut();
                
                return;
            }
            
            var didSeePlayerEnterHidable = _stateReference.value == treeStates.Find(x => x.state == State.AggroInspect).stateIndex;
            if(_canSeePlayer && !_looseSightCoroutineRunning && !didSeePlayerEnterHidable)
            {
                _inspectLocationReference.value = lastKnownPosition;
                _loseSightCoroutine = StartCoroutine(LoseSight());
                _looseSightCoroutineRunning = true;
            }
        }
        
        private IEnumerator LoseSight()
        {
            yield return new WaitForSeconds(2f);
            
            GameManager.Instance.SoundSystem.PlaySting(lostSound);
            FindFirstObjectByType<PlayerController>().GetComponent<ISightEntity>().IsSeen = false;
            _material.color = idleColour;
            
            var volume = FindFirstObjectByType<Volume>();
            var vignette = volume.sharedProfile.components[0] as Vignette;
            
            if(_activeVignetteTween != null) _activeVignetteTween.Cancel();
            _activeVignetteTween = volume.TweenValueFloat(0f, 1f, value =>
            {
                if (vignette != null) vignette.intensity.value = value;
            }).SetFrom(vignette.intensity.value).SetEaseSineInOut();
            
            var aberration = volume.sharedProfile.components[1] as ChromaticAberration;
            if(_activeAberrationTween != null) _activeAberrationTween.Cancel();
            _activeAberrationTween = volume.TweenValueFloat(0f, 1f, value =>
            {
                if (aberration != null) aberration.intensity.value = value;
            }).SetFrom(aberration.intensity.value).SetEaseSineInOut();

            _material.color = idleColour;
            SetState(State.Idle);
            
            _loseSightCoroutine = null;
            _looseSightCoroutineRunning = false;
            _canSeePlayer = false;
        }

        private IEnumerator GainSight()
        {
            var volume = FindFirstObjectByType<Volume>();
            var vignette = volume.sharedProfile.components[0] as Vignette;
            
            if(_activeVignetteTween != null) _activeVignetteTween.Cancel();
            _activeVignetteTween = volume.TweenValueFloat(0.3f, 1.4f, value =>
            {
                if (vignette != null) vignette.intensity.value = value;
            }).SetFrom(vignette.intensity.value).SetEaseSineInOut();
            
            if(_currentState == State.InspectPoint) yield return new WaitForSeconds(0);
            else yield return new WaitForSeconds(1.2f);
            
            var aberration = volume.sharedProfile.components[1] as ChromaticAberration;
            if(_activeAberrationTween != null) _activeAberrationTween.Cancel();
            _activeAberrationTween = volume.TweenValueFloat(0.8f, 1f, value =>
            {
                if (aberration != null) aberration.intensity.value = value;
            }).SetFrom(0).SetEaseSineInOut();
            
            GameManager.Instance.SoundSystem.PlaySting(detectedSound);
            
            SetState(State.AggroChase);
            _canSeePlayer = true;
            _material.color = aggroColour;
                
            OnSightEnterAction?.Invoke();
            _gainSightCoroutine = null;
            _gainSightCoroutineRunning = false;
        }

        // ====================== Public Methods ======================
        
        public BlackboardKey<T> FindBlackboardKey<T>(string keyName)
        {
            return behaviourTree ? behaviourTree.blackboard.Find<T>(keyName) : null;
        }
        
        public Vector2 DirFromAngle(float angleDeg)
        {
            angleDeg += _context.agent.transform.eulerAngles.z;
            return new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad));
        }
        
        public void PlayFootstepSound()
        {
            var sound = footstepSounds[Random.Range(0, footstepSounds.Count)];
            GameManager.Instance.SoundSystem.PlayOneShot(sound, audioSource);
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
        
        private Context CreateBehaviourTreeContext(BehaviourTree tree) 
        {
            return Context.CreateFromGameObject(gameObject, tree);
        }

        private void SetupReferences()
        {
            _stateReference = FindBlackboardKey<int>("ActiveState");
            _inspectLocationReference = FindBlackboardKey<Vector2>("InspectPoint");
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
        
        public Vector2 GetLookDirection()
        {
            return new Vector2(_animator.GetFloat(MoveX), _animator.GetFloat(MoveY));
        }

        private bool StateOverride()
        {
            return _currentState is State.AggroInspect or State.AggroChase or State.Idle;
        }
        
        // ====================== Save System ======================
        
        public string LoadData(SaveGame save)
        {
            if(!gameObject.activeSelf) return monster.ToString();
            if (!save.monsterData.ContainsKey(monster.ToString())) return monster.ToString();
            
            SetupReferences();
            var monsterSave = save.monsterData[monster.ToString()];
            gameObject.transform.parent.gameObject.SetActive(monsterSave.isActive);
            transform.position = _navMeshAgent.transform.position;
            _stateReference.value = monsterSave.activeState;
            
            return monster.ToString();
        }

        public void SaveData(SaveGame save)
        {
            if(!save.monsterData.ContainsKey(monster.ToString()))
            {
                Debug.LogError("AIManager: " + monster + " not found in save data!");
                return;
            }
            
            var monsterSave = save.monsterData[monster.ToString()];
            monsterSave.isActive = gameObject.transform.parent.gameObject.activeSelf;
            monsterSave.position = _navMeshAgent.transform.position;
            monsterSave.activeState = _stateReference.value;
        }
    }
}