/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.BehaviourTree;
using Runtime.Player;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using UnityEngine;

namespace Runtime.AI
{
    public class AIManager : MonoBehaviour, IPersistant
    {
        [SerializeField] public string persistentID;
        [SerializeField] private float menaceGaugeValue;
        [SerializeField] private float menaceGaugeMax = 100f;
        [SerializeField] private float menaceGaugeMin;
        
        [SerializeField] private float menaceGaugeIncreaseMultiplier = 1f;

        private BehaviourTreeOwner _monster;
        private PlayerController _player;
        ///Patrol state (False | Close State --- True | Far State)
        private BlackboardKey<bool> _patrolState;
        
        private bool _debugCloseState;
        public bool DebugCloseState
        {
            get => _debugCloseState;
            set {
                if (_debugCloseState == value) return;
                _debugCloseState = value;
                OnCloseStateChange?.Invoke(_debugCloseState);
            }
        }
        public delegate void OnVariableChangeDelegate(bool newVal);
        public event OnVariableChangeDelegate OnCloseStateChange;

        private void Start()
        {
            OnCloseStateChange += VariableChangeHandler;
        }

        private void VariableChangeHandler(bool newVal)
        {
            _patrolState.value = newVal;
        }
                
        // ============================ Unity Events ============================
        
        private void Awake()
        {
            _monster = FindFirstObjectByType<BehaviourTreeOwner>(FindObjectsInactive.Include);
            _player = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);

            if (_monster == null || _player == null)
            {
                Debug.LogError("AIManager: Monster or Player not found!");
                return;
            }
            
            _patrolState = _monster.FindBlackboardKey<bool>("PatrolState");
        }
        
        private void FixedUpdate()
        {
            /*
             Whether the creature is within a short walking distance of the player.
            Whether the player has actual line of sight on the alien.
             */
            
            if (_monster == null || _player == null) return;
            
            var distance = Vector3.Distance(_monster.transform.position, _player.transform.position);
            if (distance < 10f)
            {
                menaceGaugeValue += Time.fixedDeltaTime * menaceGaugeIncreaseMultiplier;
            }
            else
            {
                menaceGaugeValue -= Time.fixedDeltaTime * menaceGaugeIncreaseMultiplier;
            }
            
            menaceGaugeValue = Mathf.Clamp(menaceGaugeValue, menaceGaugeMin, menaceGaugeMax);
            
            // if (menaceGaugeValue >= menaceGaugeMax)
            // {
            //     _monster.SetActiveState();
            // }
            // else
            // {
            //     _monster.GetComponent<BehaviourTreeOwner>().SetVariableValue("IsPlayerInSight", false);
            // }
        }
        
        // ============================ Public Methods ============================
        
        public void SetMonsterState()
        {
            
        }
        
        // ============================ Save System ============================

        public void LoadData(SaveData data)
        {
            //TODO: Load data
        }

        public void SaveData(SaveData data)
        {
            //TODO: Save data
        }
    }
}
