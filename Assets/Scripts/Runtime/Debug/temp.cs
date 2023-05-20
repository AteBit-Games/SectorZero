/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using Runtime.InputSystem;
using Runtime.Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.Player
{
    public class Temp : MonoBehaviour
    {
        private NavMeshAgent _agent;
        
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            _agent.SetDestination(new Vector3(-14.1f, -45.7f, 0));
        }
    }
}