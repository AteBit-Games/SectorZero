/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
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
    public class BehaviourTreeOwner : MonoBehaviour, IPersistant
    {
        [Tooltip("BehaviourTree asset to instantiate during Awake")] 
        public BehaviourTree behaviourTree;
        public Monster monster;

        [Tooltip("Override blackboard values from the behaviour tree asset")]
        public List<BlackboardKeyValuePair> blackboardOverrides = new();
        public List<TreeState> treeStates = new();
        
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<Sound> footstepSounds;
        
        private State _currentState = State.Patrol;
        private Context _context;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        
        private BlackboardKey<int> _stateReference;
        private BlackboardKey<Vector2> _inspectLocationReference;
        private static readonly int MoveX = Animator.StringToHash("moveX");
        private static readonly int MoveY = Animator.StringToHash("moveY");
        private static readonly int IsMoving = Animator.StringToHash("isMoving");
        
        [SerializeField] private List<GameObject> initialSentinels;
        [SerializeField] private List<Collider2D> initialPatrolRooms;
        
        private BlackboardKey<List<GameObject>> _sentinelsReference;
        private BlackboardKey<List<Collider2D>> _patrolRoomsReference;
        
        private void Awake() 
        {
            var isValid = ValidateTree();
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
            
            _navMeshAgent = _context.agent;
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            _animator = GetComponent<Animator>();
            
            _sentinelsReference = FindBlackboardKey<List<GameObject>>("Sentinels");
            _patrolRoomsReference = FindBlackboardKey<List<Collider2D>>("Rooms");
            
            AddPatrolRooms(initialPatrolRooms);
            AddSentinels(initialSentinels);
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
        
        public void SetHeard(Vector2 position)
        {
            _inspectLocationReference.value = position;
            SetState(State.InspectPoint);
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

        // ====================== Public Methods ======================
        
        public BlackboardKey<T> FindBlackboardKey<T>(string keyName)
        {
            return behaviourTree ? behaviourTree.blackboard.Find<T>(keyName) : null;
        }
        
        
        public void PlayFootstepSound()
        {
            var sound = footstepSounds[Random.Range(0, footstepSounds.Count)];
            GameManager.Instance.SoundSystem.PlayOneShot(sound, audioSource);
        }

        public bool DidSeeEnter()
        {
            return _stateReference.value == treeStates.Find(x => x.state == State.AggroInspect).stateIndex;
        }
        
        public void SetLastKnownPosition(Vector2 position)
        {
            _inspectLocationReference.value = position;
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

        public bool StateOverride()
        {
            return _currentState is State.AggroInspect or State.AggroChase or State.Idle;
        }
        
        public void AddSentinels(List<GameObject> newSentinels)
        {
            Debug.Log("Adding sentinels");
            
            var sentinels = _sentinelsReference.value;
            foreach (var sentinel in newSentinels.Where(sentinel => !sentinels.Contains(sentinel)))
            {
                sentinels.Add(sentinel);
            }
            _sentinelsReference.value = sentinels;
        }
        
        public void AddPatrolRooms(List<Collider2D> newRooms)
        {
            Debug.Log("Adding patrol rooms");
            
            var rooms = _patrolRoomsReference.value;
            foreach (var room in newRooms.Where(room => !rooms.Contains(room)))
            {
                rooms.Add(room);
            }
            _patrolRoomsReference.value = rooms;
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