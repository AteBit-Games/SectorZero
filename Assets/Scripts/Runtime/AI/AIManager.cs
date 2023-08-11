/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.BehaviourTree;
using Runtime.Player;
using UnityEngine;

namespace Runtime.AI
{
    public class AIManager : MonoBehaviour
    {
        [SerializeField] private float menaceGaugeValue;
        [SerializeField] private float menaceGaugeMax = 100f;
        [SerializeField] private float menaceGaugeMin;
        
        [SerializeField] private float menaceGaugeIncreaseMultiplier = 1f;
        
        private BehaviourTreeOwner _monster;
        private PlayerController _player;
        
        // ============================ Unity Events ============================
        
        private void Awake()
        {
            _monster = FindFirstObjectByType<BehaviourTreeOwner>(FindObjectsInactive.Include);
            _player = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);
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
    }
}
