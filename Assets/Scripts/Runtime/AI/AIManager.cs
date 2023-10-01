/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using System.Collections.Generic;
using Runtime.BehaviourTree;
using Runtime.BehaviourTree.Monsters;
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

        public Action activateEvents;

        
        [HideInInspector] public VoidMask monster;
        private AIPerception _perception;
        private PlayerController _player;
        
        //------- Menace Gauge -------//
        [HideInInspector] public float menaceGaugeValue = 60f;
        [HideInInspector] public bool menaceState;
        private NavMeshPath _path;

        //Aggro level is a value between 0 and 10 that represents how impatient the monster is
        private float _lastSeenPlayerTime;
        
        //------- Blackboard Keys -------//
        ///Patrol state (True | Close State --- True | Far State)
        private BlackboardKey<bool> _patrolStateKey;
        private BlackboardKey<int> _aggroLevelKey;
        private BlackboardKey<bool> _isActiveKey;
        private BlackboardKey<Collider2D> _roomKey;
        
        //VoidMask variables
        private List<Sentinel> _sentinels;
        private BlackboardKey<int> _activeSentinelsKey;
        private BlackboardKey<float> _sentinelDurationKey;

        private bool _active;
        [HideInInspector] public bool isPlayerCrouching;

        //----- Interface ---//
        public int AggroLevel { get; private set; }

        // ============================ Unity Events ============================
        
        private void Start()
        {
            _lastSeenPlayerTime = Time.time;
            _path = new NavMeshPath();
        }
        
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
            
            if (monster == null || _player == null)
            {
                Debug.LogError("AIManager: Monster or Player not found!");
                return;
            }

            _perception.OnSightEnterAction += ResetAggroLevel;
            _sentinels = new List<Sentinel>(FindObjectsByType<Sentinel>(FindObjectsSortMode.None));
            foreach (var sentinel in _sentinels) sentinel.OnSightEnterAction += IncreaseAggroLevel;
                
            _activeSentinelsKey = monster.FindBlackboardKey<int>("ActiveSentinels");
            _sentinelDurationKey = monster.FindBlackboardKey<float>("SentinelActivationTime");
        }

        private void FixedUpdate()
        {
            if(!_active) return;
            
            if (monster == null || _player == null) return;

            var monsterPosition = monster.transform.position;
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
                    _perception.PlayerMask | _perception.WallMask);
                if (ray.collider != null)
                {
                    lineOfSight = (_perception.PlayerMask.value & 1 << ray.transform.gameObject.layer) > 0 && ray.collider.CompareTag("Player");
                }
            }

            //Patrol Close
            if (menaceState)
            {
                menaceGaugeValue += Time.deltaTime * (lineOfSight ? directSightMultiplier : directSightBase);
                menaceGaugeValue += Time.deltaTime * (totalDistance < 30f ? closeDistanceMultiplier : closeDistanceBase);
            }
            //Patrol Far
            else
            {
                menaceGaugeValue -= Time.deltaTime * Math.Clamp(AggroLevel / 8f, 0.4f, 0.95f);
            }
            
            menaceGaugeValue = Mathf.Clamp(menaceGaugeValue, menaceGaugeMin, menaceGaugeMax);
            
            if (Time.time - _lastSeenPlayerTime > 60f)
            {
                AggroLevel++;
                _aggroLevelKey.value = AggroLevel;
                _lastSeenPlayerTime = Time.time;
                UpdateSentinelAggro(AggroLevel);
            }
            
            //Flip flop between patrol states based on menace value
            if (menaceGaugeValue >= menaceGaugeMax)
            {
                SetPatrolState(false);
                menaceState = false;
            }

            if (menaceGaugeValue <= menaceGaugeMin)
            {
                SetPatrolState(true);
                menaceState = true;
            }
        }
        
        // ============================ Public Methods ============================
        
        public void AddSentinels(List<GameObject> sentinels)
        {
            
            monster.AddSentinels(sentinels);
        }
        
        public void AddRooms(List<Collider2D> rooms)
        {
            monster.AddPatrolRooms(rooms);
        }
        
        public void StartNewGame()
        {
            menaceGaugeValue = 50f;
            menaceState = false;
            _lastSeenPlayerTime = Time.time;
            AggroLevel = 0;
            _active = false;
            menaceState = false;
        }
        
        // ============================ Private Methods ============================

        private void UpdateSentinelAggro(int level)
        {
            int activeSentinels;
            float sentinelDuration;
            
            switch (level)
            {
                case <= 2:
                    activeSentinels = 4;
                    sentinelDuration = 20f;
                    break;
                case <= 4:
                    activeSentinels = 5;
                    sentinelDuration = 24f;
                    break;
                case <= 6:
                    activeSentinels = 7;
                    sentinelDuration = 30f;
                    break;
                case > 6:
                    activeSentinels = 8;
                    sentinelDuration = 38f;
                    break;
            }
            
            _activeSentinelsKey.value = activeSentinels;
            _sentinelDurationKey.value = sentinelDuration;
        }

        public void Activate()
        {
            _active = true;
            _isActiveKey.value = true;
            menaceState = false;
            _patrolStateKey.value = false;
            activateEvents?.Invoke();
        }

        private void ResetAggroLevel()
        {
            _lastSeenPlayerTime = Time.time;
            AggroLevel = 0;
            _aggroLevelKey.value = 0;
        }
        
        private void IncreaseAggroLevel(Collider2D room)
        {
            if(_active)
            {
                AggroLevel += 2;
                _aggroLevelKey.value = AggroLevel;
                menaceGaugeValue = Mathf.Clamp(menaceGaugeValue + (menaceState ? 20f : -20f), menaceGaugeMin, menaceGaugeMax);
                
                _roomKey.value = room;
                monster.SetState(MonsterState.SentinelAlert);
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
            monster = FindFirstObjectByType<VoidMask>(FindObjectsInactive.Include);
            _perception = monster.gameObject.transform.parent.GetComponentInChildren<AIPerception>();
            
            _patrolStateKey = monster.FindBlackboardKey<bool>("PatrolState");
            _aggroLevelKey = monster.FindBlackboardKey<int>("AggroLevel");
            _isActiveKey = monster.FindBlackboardKey<bool>("Active");
            _roomKey = monster.FindBlackboardKey<Collider2D>("InspectRoom");
        }
        
        // ============================ Save System ============================

        public string LoadData(SaveGame save)
        {
            if(GameManager.Instance.isMainMenu || SceneManager.GetActiveScene().name == "Tutorial") return "AIManager";
            
            InitializeReferences();
            var monsterSave = save.monsterData;
            
            _active = monsterSave.isActive;
            menaceGaugeValue = monsterSave.menaceGaugeValue;
            menaceState = monsterSave.menaceState;
            _patrolStateKey.value = menaceState;

            AggroLevel = monsterSave.aggroLevel;
            _lastSeenPlayerTime = monsterSave.lastSeenPlayerTime;

            return "AIManager";
        }

        public void SaveData(SaveGame save)
        {
            if(save.isDataSaved) return;
            if(GameManager.Instance.isMainMenu || SceneManager.GetActiveScene().name == "Tutorial") return;
            
            var monsterSave = save.monsterData;
            monsterSave.isActive = _active;
            Debug.LogError("Active: "+ _active);
            monsterSave.menaceGaugeValue = menaceGaugeValue;
            Debug.LogError("Guage Value: "+menaceGaugeValue);
            monsterSave.menaceState = menaceState;
            Debug.LogError("Menace State: "+menaceState);
            monsterSave.aggroLevel = AggroLevel;
            Debug.LogError("Aggro Level: "+AggroLevel);
            monsterSave.lastSeenPlayerTime = _lastSeenPlayerTime;
            Debug.LogError("Last Save: "+_lastSeenPlayerTime);
            
            save.isDataSaved = true;
        }
    }
}
