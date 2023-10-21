/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using System.Collections;
using Runtime.Managers;
using Runtime.Player;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.Misc.Triggers
{
    public enum Wing
    {
        OfficeWing,
        ResidentialWing,
        MedicalWing,
        SecurityWing, 
        TestingWing
    }

    [Serializable]
    public class AmbienceWing
    {
        public Wing wing;
        public Sound ambience;
    }
    
    [RequireComponent(typeof(Collider2D))]
    public class AmbienceTrigger : MonoBehaviour
    {
        [SerializeField] private AmbienceWing[] wings = new AmbienceWing[2];
        
        private Coroutine _ambienceCoroutine;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("PlayerInteraction"))
            {
                if (_ambienceCoroutine != null)
                {
                    StopCoroutine(_ambienceCoroutine);
                    _ambienceCoroutine = null;
                }
                else
                {
                    var playerAudio = other.GetComponent<PlayerAudio>();
                    if(playerAudio == null) playerAudio = other.transform.parent.GetComponent<PlayerAudio>();
                
                    //get the wing that is not the current wing from the 2 wings
                    var currentWing = playerAudio.activeWing;
                    var newWing = wings[0].wing == currentWing ? wings[1] : wings[0];
                    _ambienceCoroutine = StartCoroutine(StartTransition(playerAudio, newWing));
                }
            }
        }

        private IEnumerator StartTransition(PlayerAudio playerAudio, AmbienceWing newWing)
        {
            yield return new WaitForSeconds(4f);

            playerAudio.activeWing = newWing.wing;
            GameManager.Instance.SoundSystem.FadeToNextAmbience(newWing.ambience, 6f);
            _ambienceCoroutine = null;
        }
    }
}
