/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.InteractionSystem.Objects
{
    public class Locker : MonoBehaviour, IInteractable, IHideable
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        public bool ContainsPlayer { get; set; }
        public Transform MonsterInspectPosition { get; }

        [SerializeField, Tooltip("The location to move the player when they enter the locker")] private Transform lockerPosition;
        [SerializeField, Tooltip("The location to move the player when the exit the locker")] private Transform revealPosition;
        
        [SerializeField] private Sprite emptyLockerSprite;
        [SerializeField] private Sprite playerLockerSprite;
        
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.SoundSystem.Play(interactSound, transform);

            if (GameManager.Instance.PlayerController.isHiding)
            {
                GameManager.Instance.PlayerController.RevealPlayer(revealPosition.position);
                ContainsPlayer = false;
                _spriteRenderer.sprite = emptyLockerSprite;
            }
            else
            {
                GameManager.Instance.PlayerController.HidePlayer(lockerPosition.position);
                ContainsPlayer = true;
                _spriteRenderer.sprite = playerLockerSprite;
            }
            
            return true;
        }
        
        public bool CanInteract()
        {
            return true;
        }
    }
}
