/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using UnityEngine;

namespace Runtime.Misc.Triggers
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class SaveTrigger : MonoBehaviour, IPersistant
    {
        [SerializeField] public string persistentID;
        private bool _triggered;

        //============================== Unity Events ==============================
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !_triggered)
            {
                gameObject.SetActive(false);
                GameManager.Instance.SaveSystem.SaveGame();
                _triggered = true;
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
