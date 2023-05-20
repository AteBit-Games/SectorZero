/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.Player;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects
{
    public class Locker : MonoBehaviour, IInteractable, IHideable
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private Transform inspectPosition;
        public Transform InspectPosition => inspectPosition;

        public bool ContainsPlayer { get; set; }
        
        [SerializeField, Tooltip("The location to move the player when they enter the locker")] private Transform hidePosition;
        [SerializeField, Tooltip("The location to move the player when the exit the locker")] private Transform revealPosition;
        
        [SerializeField] private Sprite emptyLockerSprite;
        [SerializeField] private Sprite hideLockerSprite;
        
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public bool OnInteract(GameObject player)
        {
            var playerController = player.GetComponentInParent<PlayerController>();
            
            if (playerController.isHiding && ContainsPlayer)
            {
                playerController.RevealPlayer(revealPosition.position);
                ContainsPlayer = false;
                _spriteRenderer.sprite = emptyLockerSprite;
            }
            else if(!playerController.isHiding)
            {
                playerController.HidePlayer(hidePosition.position);
                ContainsPlayer = true;
                _spriteRenderer.sprite = hideLockerSprite;
            }
            else
            {
                return false;
            }
            
            GameManager.Instance.SoundSystem.Play(interactSound, transform);
            return true;
        }
        
        public bool CanInteract()
        {
            return true;
        }
    }
}
