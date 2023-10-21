/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using Runtime.BehaviourTree.Monsters;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects
{
    public class VentTrigger : MonoBehaviour
    {
        private Vent vent;

        private void Awake()
        {
            vent = GetComponentInParent<Vent>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Monster") && vent.hasPlayer && vent.progress < 0.4f)
            {
                vent.CancelMovePlayer();
                other.transform.parent.GetComponentInChildren<VoidMask>().SetState(MonsterState.KillState);
            }
        }
    }
}
