/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Misc.Triggers
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CollisionTrigger : MonoBehaviour, IPersistant
    {
        [SerializeField] public string persistentID;
        [SerializeField] protected UnityEvent triggerEvent;
        [SerializeField] protected bool triggerOnce = true;

        //============================== Unity Events ==============================
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && triggerOnce)
            {
                triggerEvent.Invoke();
                gameObject.SetActive(false);
            }
        }
        
        //============================== Save System ==============================

        public string LoadData(SaveGame game)
        {
            if (game.worldData.triggers.TryGetValue(persistentID, out var trigger))
            {
                gameObject.SetActive(value: trigger);
            }
            
            return persistentID;
        }

        public void SaveData(SaveGame game)
        {
            game.worldData.triggers[persistentID] = gameObject.activeSelf;
        }
    }
}
