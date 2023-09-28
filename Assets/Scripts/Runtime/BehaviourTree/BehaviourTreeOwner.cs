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
    public enum MonsterState
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
        [FormerlySerializedAs("state")] public MonsterState monsterState;
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
        
        private MonsterState _currentMonsterState = MonsterState.Patrol;
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
        [SerializeField] private List<GameObject> savePoints;
        
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
            SetState(MonsterState.InspectPoint);
        }

        public void SetState(MonsterState monsterState)
        {
            if(treeStates.Find(x => x.monsterState == monsterState) == null) Debug.LogError( monsterState + " state not found");
            else
            {
                _currentMonsterState = monsterState;
                SetActiveState(treeStates.Find(x => x.monsterState == monsterState).stateIndex);
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
            return _stateReference.value == treeStates.Find(x => x.monsterState == MonsterState.AggroInspect).stateIndex;
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
            return _currentMonsterState is MonsterState.AggroInspect or MonsterState.AggroChase or MonsterState.Idle;
        }
        
        public void AddSentinels(List<GameObject> newSentinels)
        {
            var sentinels = _sentinelsReference.value;
            foreach (var sentinel in newSentinels.Where(sentinel => !sentinels.Contains(sentinel)))
            {
                sentinels.Add(sentinel);
            }
            _sentinelsReference.value = sentinels;
        }
        
        public void AddPatrolRooms(List<Collider2D> newRooms)
        {
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
            Debug.Log(monsterSave.position);
            _navMeshAgent.Warp(monsterSave.position);
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
            
            //if ! aggro state, save position
            if (_currentMonsterState != MonsterState.AggroChase)
            {
                monsterSave.position = _navMeshAgent.transform.position;
                monsterSave.activeState = _stateReference.value;
            }
            else
            {
                var nearestSavePoint = savePoints[0];
                var nearestDistance = Vector2.Distance(_navMeshAgent.transform.position, nearestSavePoint.transform.position);
                foreach (var savePoint in savePoints)
                {
                    var distance = Vector2.Distance(_navMeshAgent.transform.position, savePoint.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestSavePoint = savePoint;
                    }
                }
                monsterSave.position = nearestSavePoint.transform.position;
                
                monsterSave.activeState = treeStates.Find(x => x.monsterState == MonsterState.Patrol).stateIndex;
            }
        }
    }
}