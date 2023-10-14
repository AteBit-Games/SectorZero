/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.Player;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects
{
    public class Vent : MonoBehaviour, IInteractable
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField, Tooltip("The location to move the player when the exit the locker")] private Transform revealPosition;
        [SerializeField] private ExitDirection exitDirection;
        
        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            var playerController = player.GetComponentInParent<PlayerController>();

            var facingDirection = exitDirection switch
            {
                ExitDirection.Left => Vector2.left,
                ExitDirection.Right => Vector2.right,
                ExitDirection.Up => Vector2.up,
                ExitDirection.Down => Vector2.down,
                _ => Vector2.zero
            };
            
            playerController.transform.position = revealPosition.position;
            if (facingDirection != Vector2.zero) playerController.SetFacingDirection(facingDirection);
            
            GameManager.Instance.SoundSystem.Play(interactSound, transform.GetComponent<AudioSource>());
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            throw new System.NotImplementedException();
        }
        
        public bool CanInteract()
        {
            return true;
        }
    }
}
