/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
using System.Collections.Generic;
using Runtime.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.AI
{
    [DefaultExecutionOrder(10)]
    public class CafeteriaSentinels : MonoBehaviour
    {
        [SerializeField] private List<Sentinel> sentinels;
        
        private AIManager _aiManager;
        
        // ============================ Unity Events ============================

        private void Awake()
        {
            _aiManager = GameManager.Instance.AIManager;
        }

        private void Start()
        {
            StartCoroutine(SentinelLoop());
        }
        
        // ============================ Private Methods ============================

        private IEnumerator SentinelLoop()
        { 
            ShuffleSentinels(); 
            var duration = GetSentinelDuration();
            //var count = GetSentinelCount();
            
           // for (var i = 0; i < count; i++) sentinels[i].ActivateSentinel(duration);
           sentinels[0].ActivateSentinel(duration);
           yield return new WaitForSeconds(duration+10);
           StartCoroutine(SentinelLoop());
        }

        private void ShuffleSentinels()
        {
            var count = sentinels.Count;
            while (count > 1) 
            {
                count--;
                var randomIndex = Random.Range(0, count + 1);
                (sentinels[randomIndex], sentinels[count]) = (sentinels[count], sentinels[randomIndex]);
            }
        }

        private int GetSentinelCount()
        {
            return _aiManager.AggroLevel switch
            {
                <= 0 => 1,
                <= 4 => 2,
                > 4 => 3
            };
        }

        private int GetSentinelDuration()
        {
            return _aiManager.AggroLevel switch
            {
                <= 0 => 16,
                <= 4 => 22,
                > 4 => 28
            };
        }
    }
}
