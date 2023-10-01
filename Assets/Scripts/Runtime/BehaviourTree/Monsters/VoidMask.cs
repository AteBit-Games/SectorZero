/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using UnityEngine;

namespace Runtime.BehaviourTree.Monsters 
{
    [DefaultExecutionOrder(10)]
    public class VoidMask : BehaviourTreeOwner, IPersistant
    {
        [SerializeField] private List<GameObject> initialSentinels;
        [SerializeField] private List<Collider2D> initialPatrolRooms;
        [SerializeField] private List<GameObject> savePoints;
        
        private BlackboardKey<List<GameObject>> _sentinelsReference;
        private BlackboardKey<List<Collider2D>> _patrolRoomsReference;
        
        protected new void Awake() 
        {
            base.Awake();
            
            _sentinelsReference = FindBlackboardKey<List<GameObject>>("Sentinels");
            _patrolRoomsReference = FindBlackboardKey<List<Collider2D>>("Rooms");

            AddPatrolRooms(initialPatrolRooms);
            AddSentinels(initialSentinels);
        }
        
        public void AddSentinels(List<GameObject> newSentinels)
        {
            var sentinels = _sentinelsReference.value;
            foreach (var sentinel in newSentinels.Where(sentinel => !sentinels.Contains(sentinel)))
            {
                sentinels.Add(sentinel);
            }
            _sentinelsReference.value = sentinels;
        }
        
        public void AddPatrolRooms(List<Collider2D> newRooms)
        {
            var rooms = _patrolRoomsReference.value;
            foreach (var room in newRooms.Where(room => !rooms.Contains(room)))
            {
                rooms.Add(room);
            }
            _patrolRoomsReference.value = rooms;
        }
        
        public string LoadData(SaveGame save)
        {
            if(!gameObject.activeSelf) return "VoidMask";
            if (!save.monsterData.ContainsKey("VoidMask")) return "VoidMask";
            
            SetupReferences();
            var monsterSave = save.monsterData["VoidMask"];
            navMeshAgent.Warp(monsterSave.position);
            SetState((MonsterState)monsterSave.activeState);

            return "VoidMask";
        }

        public void SaveData(SaveGame save)
        {
            if(!save.monsterData.ContainsKey("VoidMask"))
            {
                Debug.LogError("AIManager: " + "VoidMask" + " not found in save data!");
                return;
            }
            
            var monsterSave = save.monsterData["VoidMask"];
            
            //if ! aggro state, save position
            if (currentMonsterState != MonsterState.AggroChase)
            {
                monsterSave.position = navMeshAgent.transform.position;
                monsterSave.activeState = stateReference.value;
            }
            else
            {
                var nearestSavePoint = savePoints[0];
                var nearestDistance = Vector2.Distance(navMeshAgent.transform.position, nearestSavePoint.transform.position);
                foreach (var savePoint in savePoints)
                {
                    var distance = Vector2.Distance(navMeshAgent.transform.position, savePoint.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestSavePoint = savePoint;
                    }
                }
                monsterSave.position = nearestSavePoint.transform.position;
                
                monsterSave.activeState = treeStates.Find(x => x.monsterState == MonsterState.Patrol).stateIndex;
            }
        }
    }
}