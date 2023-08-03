/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Misc.Triggers
{
    public class CollisionTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent triggerEvent;
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                triggerEvent.Invoke();
            }
        }
    }
}
