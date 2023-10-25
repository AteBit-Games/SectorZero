/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using UnityEngine;


namespace Runtime.Misc.Triggers
{
    public class Trigger : MonoBehaviour
    {
        [SerializeField] private SoundSystem.AmbienceTrigger ambienceTrigger;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.CompareTag("Player"))
            {
                ambienceTrigger.TriggerAmbience(6);
            }
        }
    }
}
