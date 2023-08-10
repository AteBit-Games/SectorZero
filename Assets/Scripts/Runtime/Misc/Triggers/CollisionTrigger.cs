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
        [SerializeField] protected UnityEvent triggerEvent;
        [SerializeField] protected bool triggerOnce = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && triggerOnce)
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
            if (data.worldData.triggers.TryGetValue(persistentID, out var trigger))
            {
                gameObject.SetActive(value: trigger);
            }
        }

        public void SaveData(SaveData data)
        {
            data.worldData.triggers[persistentID] = gameObject.activeSelf;
        }
    }
}
