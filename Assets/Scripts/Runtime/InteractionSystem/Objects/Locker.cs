/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.Managers;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.InteractionSystem
{
    public class Locker : MonoBehaviour, IInteractable
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;

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
                _spriteRenderer.sprite = emptyLockerSprite;
            }
            else
            {
                GameManager.Instance.PlayerController.HidePlayer(lockerPosition.position);
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
