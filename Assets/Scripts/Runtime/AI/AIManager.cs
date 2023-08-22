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
        [SerializeField] private float closeDistanceMultiplier = 0.3f;

        private BehaviourTreeOwner _monster;
        private PlayerController _player;
        
        //------- Menace Gauge -------//
        private float _menaceGaugeValue;
        private bool _menaceState = true;
        private NavMeshPath _path;

        //Aggro level is a value between 0 and 10 that represents how impatient the monster is
        private int _aggroLevel;
        private float _lastSeenPlayerTime;
        
        //------- Blackboard Keys -------//
        ///Patrol state (True | Close State --- True | Far State)
        private BlackboardKey<bool> _patrolStateKey;
        private BlackboardKey<int> _aggroLevelKey;
        
        //VoidMask variables
        private List<Sentinel> _sentinels;
        private BlackboardKey<int> _activeSentinelsKey;
        private BlackboardKey<float> _sentinelDurationKey;

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
            
            _player = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);
            _monster = FindFirstObjectByType<BehaviourTreeOwner>(FindObjectsInactive.Include);
            
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
            
            _patrolStateKey = _monster.FindBlackboardKey<bool>("PatrolState");
            _aggroLevelKey = _monster.FindBlackboardKey<int>("AggroLevel");
            
            //reset all values
            _menaceGaugeValue = 0f;
            _menaceState = true;
            _aggroLevel = 0;
            
            _aggroLevelKey.value = 0;
            _patrolStateKey.value = true;
        }

        private void FixedUpdate()
        {
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
                _menaceGaugeValue += Time.deltaTime * (lineOfSight ? directSightMultiplier : 0.15f);
                _menaceGaugeValue += Time.deltaTime * (totalDistance < 30f ? closeDistanceMultiplier : 0.15f);
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

        private void ResetAggroLevel()
        {
            _lastSeenPlayerTime = Time.time;
            _aggroLevel = 0;
            _aggroLevelKey.value = 0;
        }
        
        private void IncreaseAggroLevel()
        {
            _aggroLevel += 2;
            _aggroLevelKey.value = _aggroLevel;
            _menaceGaugeValue = Mathf.Clamp(_menaceGaugeValue + (_menaceState ? 20f : -20f), menaceGaugeMin, menaceGaugeMax);
        }
        
        private void SetPatrolState(bool state)
        {
            _patrolStateKey.value = state;
        }
        
        // ============================ Save System ============================

        public void LoadData(SaveGame save)
        {
            _menaceGaugeValue = save.monsterData.menaceGaugeValue;
            _menaceState = save.monsterData.menaceState;
            _aggroLevel = save.monsterData.aggroLevel;
            _lastSeenPlayerTime = save.monsterData.lastSeenPlayerTime;
        }

        public void SaveData(SaveGame save)
        {
            save.monsterData.menaceGaugeValue = _menaceGaugeValue;
            save.monsterData.menaceState = _menaceState;
            save.monsterData.aggroLevel = _aggroLevel;
            save.monsterData.lastSeenPlayerTime = _lastSeenPlayerTime;
        }
    }
}
