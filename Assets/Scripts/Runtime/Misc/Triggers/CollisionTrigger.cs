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
    public class CollisionTrigger : MonoBehaviour, IPersistant
    {
        [SerializeField] private UnityEvent triggerEvent;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                triggerEvent.Invoke();
                gameObject.SetActive(false);
            }
        }

        [SerializeField] private string persistentID;
        public string ID
        {
            get => persistentID;
            set => persistentID = value;
        }
        
        public void LoadData(SaveData data)
        {
            if (data.worldData.triggers.ContainsKey(persistentID))
            {
                gameObject.SetActive(data.worldData.triggers[persistentID]);
            }
        }

        public void SaveData(SaveData data)
        {
            if(!data.worldData.triggers.ContainsKey(persistentID)) data.worldData.triggers.Add(persistentID, gameObject.activeSelf);
            else data.worldData.triggers[persistentID] = gameObject.activeSelf;
        }
    }
}
