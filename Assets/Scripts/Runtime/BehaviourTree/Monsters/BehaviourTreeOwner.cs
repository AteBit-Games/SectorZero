/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using System.Collections.Generic;
using Runtime.Managers;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Runtime.BehaviourTree.Monsters 
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
    
    [AddComponentMenu("BehaviourTree/BehaviourTreeOwner")]
    [DefaultExecutionOrder(10)]
    public class BehaviourTreeOwner : MonoBehaviour
    {
        [Tooltip("BehaviourTree asset to instantiate during Awake")] 
        public BehaviourTree behaviourTree;

        [Tooltip("Override blackboard values from the behaviour tree asset")]
        public List<BlackboardKeyValuePair> blackboardOverrides = new();
        public List<TreeState> treeStates = new();
        
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<Sound> footstepSounds;
        
        [HideInInspector] public MonsterState currentMonsterState;
        protected NavMeshAgent navMeshAgent;
        private Context _context;
        private Animator _animator;
        
        protected BlackboardKey<int> stateReference;
        private BlackboardKey<Vector2> _inspectLocationReference;
        private static readonly int MoveX = Animator.StringToHash("moveX");
        private static readonly int MoveY = Animator.StringToHash("moveY");
        private static readonly int IsMoving = Animator.StringToHash("isMoving");
        
        protected void Awake() 
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
            
            navMeshAgent = _context.agent;
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
            _animator = GetComponent<Animator>();
        }
        
        public void SetActiveState(int state)
        {
            stateReference.value = state;
        }
        
        protected void Start()
        {
            behaviourTree.treeState = Node.State.Running;
            if(GameManager.Instance.TestMode) SetState(MonsterState.Idle);
        }
        
        protected void Update() 
        {
            if (behaviourTree) behaviourTree.Update();
            _animator.transform.position = navMeshAgent.transform.position;
            _animator.SetBool(IsMoving, navMeshAgent.velocity.magnitude > 0.1f);
        }

        protected void OnDrawGizmos() 
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
                currentMonsterState = monsterState;
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
            return stateReference.value == treeStates.Find(x => x.monsterState == MonsterState.AggroInspect).stateIndex;
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

        protected void SetupReferences()
        {
            stateReference = FindBlackboardKey<int>("ActiveState");
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
            return currentMonsterState is MonsterState.AggroInspect or MonsterState.AggroChase or MonsterState.Idle;
        }
        
    }
}