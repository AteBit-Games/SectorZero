/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.Player;
using UnityEngine;

namespace Runtime.Misc.Triggers
{
    public class TutorialTrigger : CollisionTrigger
    {
        [SerializeField] private string stageTrigger;

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<PlayerController>();
                if (player.IsSneaking)
                {
                    triggerEvent.Invoke();
                    gameObject.SetActive(false);
                }
            }
        }
        
    }
}