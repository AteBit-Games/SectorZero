/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
using System.Collections.Generic;
using Runtime.BehaviourTree;
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
        [SerializeField] private BehaviourTreeOwner behaviourTreeOwner;

        private bool _hasAdded;

        private void Start()
        {
            Debug.Assert(behaviourTreeOwner != null, "behaviourTreeOwner != null");
            Debug.Assert(sentinels.Count > 0, "sentinels.Count > 0");
            
            GameManager.Instance.AIManager.activateEvents += AddSentinels;
        }

        private void AddSentinels()
        {
            _hasAdded = true;
            StartCoroutine(AddSentinelsRoutine());
        }

        private IEnumerator AddSentinelsRoutine()
        {
            yield return new WaitForSeconds(30f);
            behaviourTreeOwner.AddSentinels(sentinels);
        }

        // ============================ Save System ============================

        public string LoadData(SaveGame game)
        {
            if (game.monsterData["VoidMask"].addedInitialSentinels)
            {
                AddSentinels();
            }
            
            return persistentID;
        }

        public void SaveData(SaveGame game)
        {
            game.monsterData["VoidMask"].addedInitialSentinels = _hasAdded;
        }
    }
}