/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using System.Collections.Generic;
using Runtime.BehaviourTree;
using Runtime.Managers;
using Runtime.Player;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace Runtime.AI
{
    public class AIManager : MonoBehaviour, IPersistant
    {
        [SerializeField] private float menaceGaugeMax = 100f;
        [SerializeField] private float menaceGaugeMin;
        
        [SerializeField] private float directSightMultiplier = 0.4f;
        [SerializeField, Tooltip("If no direct sight, what is the base value")] private float directSightBase = 0.15f;
        [SerializeField] private float closeDistanceMultiplier = 0.3f;
        [SerializeField, Tooltip("If not close enough, what is the base value")] private float closeDistanceBase = 0.15f;

        private BehaviourTreeOwner _monster;
        private PlayerController _player;
        
        //------- Menace Gauge -------//
        private float _menaceGaugeValue = 45f;
        private bool _menaceState;
        private NavMeshPath _path;

        //Aggro level is a value between 0 and 10 that represents how impatient the monster is
        private int _aggroLevel;
        private float _lastSeenPlayerTime;
        
        //------- Blackboard Keys -------//
        ///Patrol state (True | Close State --- True | Far State)
        private BlackboardKey<bool> _patrolStateKey;
        private BlackboardKey<int> _aggroLevelKey;
        private BlackboardKey<bool> _isActiveKey;
        
        //VoidMask variables
        private List<Sentinel> _sentinels;
        private BlackboardKey<int> _activeSentinelsKey;
        private BlackboardKey<float> _sentinelDurationKey;

        private bool _active;

        private void Start()
        {
            _lastSeenPlayerTime = Time.time;
            _path = new NavMeshPath();
        }
        
        // ============================ Unity Events ============================
        
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if(GameManager.Instance.isMainMenu || SceneManager.GetActiveScene().name == "Tutorial") return;
            
            InitializeReferences();
            
            if (_monster == null || _player == null)
            {
                Debug.LogError("AIManager: Monster or Player not found!");
                return;
            }

            _monster.OnSightEnterAction += ResetAggroLevel;
            if (_monster.monster == Monster.VoidMask)
            {
                _sentinels = new List<Sentinel>(FindObjectsByType<Sentinel>(FindObjectsSortMode.None));
                foreach (var sentinel in _sentinels) sentinel.OnSightEnterAction += IncreaseAggroLevel;
                
                _activeSentinelsKey = _monster.FindBlackboardKey<int>("ActiveSentinels");
                _sentinelDurationKey = _monster.FindBlackboardKey<float>("SentinelActivationTime");
            }
        }

        private void FixedUpdate()
        {
            if(!_active) return;
            
            if (_monster == null || _player == null) return;

            var monsterPosition = _monster.transform.position;
            var playerPosition = _player.transform.position;

            NavMesh.CalculatePath(monsterPosition, playerPosition, NavMesh.AllAreas, _path);
            var totalDistance = 0f;
            if (_path.status == NavMeshPathStatus.PathComplete)
            {
                var previousCorner = monsterPosition;
                foreach (var corner in _path.corners)
                {
                    totalDistance += Vector3.Distance(previousCorner, corner);
                    previousCorner = corner;
                }
            }
            else totalDistance = 999f;
            
            var lineOfSight = false;
            if(totalDistance < 30f)
            {
                var rayDistance = Vector3.Distance(monsterPosition, playerPosition);
                var ray = Physics2D.Raycast(monsterPosition, playerPosition - monsterPosition, rayDistance,
                    _monster.PlayerMask | _monster.WallMask);
                if (ray.collider != null)
                {
                    lineOfSight = (_monster.PlayerMask.value & 1 << ray.transform.gameObject.layer) > 0 && ray.collider.CompareTag("Player");
                }
            }

            //Patrol Close
            if (_menaceState)
            {
                _menaceGaugeValue += Time.deltaTime * (lineOfSight ? directSightMultiplier : directSightBase);
                _menaceGaugeValue += Time.deltaTime * (totalDistance < 30f ? closeDistanceMultiplier : closeDistanceBase);
            }
            //Patrol Far
            else
            {
                _menaceGaugeValue -= Time.deltaTime * Math.Clamp(_aggroLevel / 8f, 0.3f, 0.8f);
            }
            
            _menaceGaugeValue = Mathf.Clamp(_menaceGaugeValue, menaceGaugeMin, menaceGaugeMax);
            
            if (Time.time - _lastSeenPlayerTime > 60f)
            {
                _aggroLevel++;
                _aggroLevelKey.value = _aggroLevel;
                _lastSeenPlayerTime = Time.time;
                
                if(_monster.monster == Monster.VoidMask) UpdateSentinelAggro(_aggroLevel);
            }
            
            //Flip flop between patrol states based on menace value
            if (_menaceGaugeValue >= menaceGaugeMax)
            {
                SetPatrolState(false);
                _menaceState = false;
            }

            if (_menaceGaugeValue <= menaceGaugeMin)
            {
                SetPatrolState(true);
                _menaceState = true;
            }
        }
        
        // ============================ Private Methods ============================

        private void UpdateSentinelAggro(int level)
        {
            var activeSentinels = 0;
            var sentinelDuration = 0f;
            
            switch (level)
            {
                case <= 3:
                    activeSentinels = 2;
                    sentinelDuration = 15f;
                    break;
                case <= 6:
                    activeSentinels = 4;
                    sentinelDuration = 22f;
                    break;
                case <= 9:
                    activeSentinels = 6;
                    sentinelDuration = 28f;
                    break;
                case 10:
                    activeSentinels = 8;
                    sentinelDuration = 35f;
                    break;
            }
            
            _activeSentinelsKey.value = activeSentinels;
            _sentinelDurationKey.value = sentinelDuration;
        }

        private void Activate()
        {
            _active = true;
            _isActiveKey.value = true;
            _menaceState = false;
            _patrolStateKey.value = false;
        }

        private void ResetAggroLevel()
        {
            _lastSeenPlayerTime = Time.time;
            _aggroLevel = 0;
            _aggroLevelKey.value = 0;
        }
        
        private void IncreaseAggroLevel()
        {
            if(_active)
            {
                _aggroLevel += 2;
                _aggroLevelKey.value = _aggroLevel;
                _menaceGaugeValue = Mathf.Clamp(_menaceGaugeValue + (_menaceState ? 20f : -20f), menaceGaugeMin, menaceGaugeMax);
            }
            else Activate();
        }
        
        private void SetPatrolState(bool state)
        {
            _patrolStateKey.value = state;
        }
        
        private void InitializeReferences()
        {
            _player = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);
            _monster = FindFirstObjectByType<BehaviourTreeOwner>(FindObjectsInactive.Include);
            
            _patrolStateKey = _monster.FindBlackboardKey<bool>("PatrolState");
            _aggroLevelKey = _monster.FindBlackboardKey<int>("AggroLevel");
            _isActiveKey = _monster.FindBlackboardKey<bool>("Active");
        }
        
        // ============================ Save System ============================

        public string LoadData(SaveGame save)
        {
            if(GameManager.Instance.isMainMenu || SceneManager.GetActiveScene().name == "Tutorial") return "AIManager";
            
            InitializeReferences();
            if (!save.monsterData.ContainsKey(_monster.monster.ToString())) return "AIManager";
            
            var monsterSave = save.monsterData[_monster.monster.ToString()];
            _menaceGaugeValue = monsterSave.menaceGaugeValue;
            _menaceState = monsterSave.menaceState;
            _patrolStateKey.value = _menaceState;

            _aggroLevel = monsterSave.aggroLevel;
            _lastSeenPlayerTime = monsterSave.lastSeenPlayerTime;

            return "AIManager";
        }

        public void SaveData(SaveGame save)
        {
            if(GameManager.Instance.isMainMenu || SceneManager.GetActiveScene().name == "Tutorial") return;
            
            var monsterSave = save.monsterData[_monster.monster.ToString()];
            if (monsterSave == null)
            {
                Debug.LogError("AIManager: " + _monster.monster + " not found in save data!");
                return;
            }
                
            monsterSave.menaceGaugeValue = _menaceGaugeValue;
            monsterSave.menaceState = _menaceState;
            monsterSave.aggroLevel = _aggroLevel;
            monsterSave.lastSeenPlayerTime = _lastSeenPlayerTime;
        }
    }
}
