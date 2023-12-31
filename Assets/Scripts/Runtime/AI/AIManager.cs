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
using Runtime.SoundSystem;
using Tweens;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Runtime.AI
{
    public struct DebugData
    {
        public string title;
        public List<string> keys;
    }

    [DefaultExecutionOrder(4)]
    public class AIManager : MonoBehaviour, IPersistant
    {
        [SerializeField] private List<Sound> graceSwapSounds;
        [SerializeField] private List<Sound> menaceSwapSounds;
        
        [SerializeField] private List<Sound> monsterStings;
        [SerializeField] private Vector2 stingInterval = new(60f, 120f);
        
        [SerializeField] private float menaceGaugeMax = 100f;
        [SerializeField] private float menaceGaugeMin;

        [SerializeField] private float directSightMultiplier = 0.4f;

        [SerializeField, Tooltip("If no direct sight, what is the base value")]
        private float directSightBase = 0.15f;

        [SerializeField] private float closeDistanceMultiplier = 0.3f;

        [SerializeField, Tooltip("If not close enough, what is the base value")]
        private float closeDistanceBase = 0.15f;

        public Action activateEvents;

        [HideInInspector] public VoidMask monster;
        private AIPerception _perception;
        private PlayerController _player;

        //------- Menace Gauge -------//
        public float currentMenaceGaugeValue = 5f;
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

        private List<DebugData> _debugData;

        private bool _active;
        [HideInInspector] public bool isPlayerCrouching;

        private Volume _volume;
        [HideInInspector] public HeartBeatSystem heartBeatSystem;
        
        //----- Adaptive Audio -----//
        private float _stingCooldown;        
        private AudioSource _stingSource;

        //----- Interface ---//
        public int AggroLevel { get; private set; }
        

        // ============================ Unity Events ============================

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            if (scene.name != "SectorTwo")
            {
                _active = false;
                return;
            }
            
            _volume = FindFirstObjectByType<Volume>();
            _lastSeenPlayerTime = Time.time;
            _path = new NavMeshPath();

            _stingCooldown = UnityEngine.Random.Range(stingInterval.x, stingInterval.y);
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

            _stingSource = GameObject.Find("StingSource").GetComponent<AudioSource>();
        }

        private void Update()
        {
            _debugData = new List<DebugData>();
        }

        public void AddData(DebugData data)
        {
            _debugData.Add(data);
        }

        public IEnumerable<DebugData> GetDebugData()
        {
            return _debugData;
        }

        public void Disable()
        {
            heartBeatSystem.Disable();
            _active = false;
        }

        private void FixedUpdate()
        {
            if (!_active) return;

            if (monster == null || _player == null) return;

            var monsterPosition = monster.transform.position;
            var playerPosition = _player.transform.position;
            
            var distance = Vector2.Distance(monsterPosition, playerPosition);
            Debug.Log(distance);

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
            if (totalDistance < 30f)
            {
                var rayDistance = Vector3.Distance(monsterPosition, playerPosition);
                var ray = Physics2D.Raycast(monsterPosition, playerPosition - monsterPosition, rayDistance,
                    _perception.PlayerMask | _perception.WallMask);
                if (ray.collider != null)
                {
                    lineOfSight = (_perception.PlayerMask.value & 1 << ray.transform.gameObject.layer) > 0 &&
                                  ray.collider.CompareTag("Player");
                }
            }
            
            if(_stingCooldown > 0f)
            {
                _stingCooldown -= Time.deltaTime;
            }
            
            if(_stingCooldown <= 0f && distance > 40f)
            {
                var sound = monsterStings[UnityEngine.Random.Range(0, monsterStings.Count)];
                GameManager.Instance.SoundSystem.Play(sound, _stingSource);
                _stingCooldown = UnityEngine.Random.Range(stingInterval.x, stingInterval.y);
            }

            //Patrol Close
            if (menaceState)
            {
                var sightAmount = Time.deltaTime * (lineOfSight ? directSightMultiplier : directSightBase);
                currentMenaceGaugeValue += sightAmount;
                
                var distanceAmount = Time.deltaTime * (totalDistance < 30f ? closeDistanceMultiplier : closeDistanceBase);
                currentMenaceGaugeValue += distanceAmount;
            }
            //Patrol Far
            else
            {
                currentMenaceGaugeValue -= Time.deltaTime * Math.Clamp(AggroLevel / 10f, 0.35f, 0.75f);
            }
            
            currentMenaceGaugeValue = Mathf.Clamp(currentMenaceGaugeValue, menaceGaugeMin, menaceGaugeMax);

            if (Time.time - _lastSeenPlayerTime > 60f)
            {
                AggroLevel = Mathf.Clamp(menaceState ? AggroLevel+1 : AggroLevel-1, 0, 10);
                _aggroLevelKey.value = AggroLevel;
                _lastSeenPlayerTime = Time.time;
                UpdateSentinelAggro(AggroLevel);
            }
            
            DetermineHeartbeat(distance);

            //Flip flop between patrol states based on menace value
            if (currentMenaceGaugeValue >= menaceGaugeMax)
            {
                SetPatrolState(false);
                menaceState = false;
                
                //pick random sound
                var sound = graceSwapSounds[UnityEngine.Random.Range(0, graceSwapSounds.Count)];
                GameManager.Instance.SoundSystem.Play(sound);

                SetFilmGrain(0.2f);
            }

            if (currentMenaceGaugeValue <= menaceGaugeMin)
            {
                SetPatrolState(true);
                menaceState = true;
                
                var sound = menaceSwapSounds[UnityEngine.Random.Range(0, menaceSwapSounds.Count)];
                GameManager.Instance.SoundSystem.Play(sound);
                
                SetFilmGrain(0.4f);
            }
        }
        
        private float _currentHeartRate;

        private void DetermineHeartbeat(float distance)
        {
            if(distance < 40f)
            {
                heartBeatSystem.Enable();

                switch (distance)
                {
                    case <= 10f:
                        if(_currentHeartRate == 110) break;
                        heartBeatSystem.SetHeartRateSmoothly(110);
                        _currentHeartRate = 110;
                        break;
                    case <= 20f:
                        if(_currentHeartRate == 90) break;
                        heartBeatSystem.SetHeartRateSmoothly(90);
                        _currentHeartRate = 90;
                        break;
                    case <= 30f:
                        if(_currentHeartRate == 75) break;
                        heartBeatSystem.SetHeartRateSmoothly(75);
                        _currentHeartRate = 75;
                        break;
                    case <= 40f:
                        if(_currentHeartRate == 60) break;
                        heartBeatSystem.SetHeartRateSmoothly(60);
                        _currentHeartRate = 60;
                        break;
                }
            }
            else
            {
                heartBeatSystem.Disable();
            }
        }

        // ============================ Public Methods ============================

        public void AddSentinels(List<GameObject> sentinels)
        {

            monster.AddSentinels(sentinels);
        }

        public void AddRooms(List<Collider2D> rooms)
        {
            if(monster == null) InitializeReferences();
            monster.AddPatrolRooms(rooms);
        }

        public void StartNewGame()
        {
            currentMenaceGaugeValue = 60f;
            menaceState = false;
            _lastSeenPlayerTime = Time.time;
            AggroLevel = 0;
            _active = false;
        }

        // ============================ Private Methods ============================

        private void UpdateSentinelAggro(int level)
        {
            int activeSentinels;
            float sentinelDuration;

            switch (level)
            {
                case <= 2:
                    activeSentinels = 3;
                    sentinelDuration = 20f;
                    break;
                case <= 4:
                    activeSentinels = 4;
                    sentinelDuration = 22f;
                    break;
                case <= 6:
                    activeSentinels = 5;
                    sentinelDuration = 25f;
                    break;
                case > 6:
                    activeSentinels = 6;
                    sentinelDuration = 28f;
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
            if (_active)
            {
                AggroLevel = Mathf.Clamp(AggroLevel + 2, 0, 10);
                _aggroLevelKey.value = AggroLevel;
                currentMenaceGaugeValue = Mathf.Clamp(currentMenaceGaugeValue + (menaceState ? 20f : -20f), menaceGaugeMin,
                    menaceGaugeMax);

                _roomKey.value = room;

                if (!monster.StateOverride())
                {
                    monster.SetState(MonsterState.Patrol);
                    monster.SetState(MonsterState.SentinelAlert);
                }
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
            heartBeatSystem = _player.GetComponentInChildren<HeartBeatSystem>();
            
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
            if (GameManager.Instance.isMainMenu || SceneManager.GetActiveScene().name != "SectorTwo") return "AIManager";

            InitializeReferences();
            var monsterSave = save.monsterData;

            _active = monsterSave.isActive;
            currentMenaceGaugeValue = monsterSave.menaceGaugeValue;
            menaceState = monsterSave.menaceState;
            _patrolStateKey.value = menaceState;

            AggroLevel = monsterSave.aggroLevel;
            _lastSeenPlayerTime = monsterSave.lastSeenPlayerTime;

            return "AIManager";
        }

        public void SaveData(SaveGame save)
        {
            if (GameManager.Instance.isMainMenu || SceneManager.GetActiveScene().name != "SectorTwo") return;
            
            var monsterSave = save.monsterData;
            monsterSave.isActive = _active;
            monsterSave.menaceGaugeValue = currentMenaceGaugeValue;
            monsterSave.menaceState = menaceState;
            monsterSave.aggroLevel = AggroLevel;
            monsterSave.lastSeenPlayerTime = _lastSeenPlayerTime;
        }

        private void SetFilmGrain(float intensity)
        {
            if (_volume.sharedProfile.components[2] is FilmGrain filmGrain)
            {
                var tween = new FloatTween
                {
                    from = filmGrain.intensity.value,
                    to = intensity,
                    duration = 0.8f,
                    easeType = EaseType.SineInOut,
                    onUpdate = (_, value) =>
                    {
                        filmGrain.intensity.value = value;
                    }
                };
                
                _volume.gameObject.AddTween(tween);
            }
        }
    }
}
