/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
using System.Collections.Generic;
using Runtime.BehaviourTree.Monsters;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using UnityEngine;

namespace Runtime.Misc.Helpers
{
    public class SentinelHelper : MonoBehaviour, IPersistant
    {
        [SerializeField] public string persistentID;
        [SerializeField] private List<GameObject> sentinels;
        [SerializeField] private VoidMask voidMask;

        private bool _hasAdded;

        private void Start()
        {
            Debug.Assert(voidMask != null, "behaviourTreeOwner != null");
            Debug.Assert(sentinels.Count > 0, "sentinels.Count > 0");
            
            GameManager.Instance.AIManager.activateEvents += AddSentinels;
        }
        
        private void OnDestroy()
        {
            GameManager.Instance.AIManager.activateEvents -= AddSentinels;
        }

        private void AddSentinels()
        {
            _hasAdded = true;
            StartCoroutine(AddSentinelsRoutine());
        }

        private IEnumerator AddSentinelsRoutine()
        {
            yield return new WaitForSeconds(30f);
            voidMask.AddSentinels(sentinels);
        }

        // ============================ Save System ============================

        public string LoadData(SaveGame game)
        {
            if (game.monsterData.addedInitialSentinels)
            {
                voidMask.AddSentinels(sentinels);
            }
            
            return persistentID;
        }

        public void SaveData(SaveGame game)
        {
            game.monsterData.addedInitialSentinels = _hasAdded;
        }
    }
}
