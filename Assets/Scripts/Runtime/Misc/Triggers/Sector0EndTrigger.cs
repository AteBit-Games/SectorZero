/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using Runtime.Managers;
using Runtime.Player;
using UnityEngine;

namespace Runtime.Misc.Triggers
{
    public class Sector0EndTrigger : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private CinematicManager cinematicManager;
        [SerializeField] private SoundSystem.AmbienceTrigger ambienceTrigger;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.CompareTag("Player"))
            {
                playerController.DisableInput();
                playerController.SetFacingDirection(Vector2.up);
                playerController.SetCamera(true);
                cinematicManager.TriggerCinematic(0);
                ambienceTrigger.TriggerAmbience(6);
            }
        }

        public void Trigger()
        {
            GameManager.Instance.SaveSystem.UpdatePlayerEnding(0);
            GameManager.Instance.EndGame();
        }
    }
}
