/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections.Generic;
using Runtime.Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Tutorial
{
    public class Trigger : MonoBehaviour
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
