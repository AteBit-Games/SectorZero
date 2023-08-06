/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.Player;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.InteractionSystem.Objects
{
    enum ExitDirection
    {
        Left,
        Right,
        Up,
        Down
    }
    
    public class Locker : MonoBehaviour, IInteractable, IHideable
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private UnityEvent onTriggerEvents;
        public UnityEvent OnInteractEvents => onTriggerEvents;
        
        [SerializeField] private Transform inspectPosition;
        public Transform InspectPosition => inspectPosition;

        public bool ContainsPlayer { get; set; }
        private bool _canExit = true;

        [SerializeField, Tooltip("The location to move the player when they enter the locker")] private Transform hidePosition;
        [SerializeField, Tooltip("The location to move the player when the exit the locker")] private Transform revealPosition;
        [SerializeField] private ExitDirection exitDirection;
        
        [SerializeField] private Sprite emptyLockerSprite;
        [SerializeField] private Sprite hideLockerSprite;
        [SerializeField] private bool canTrigger;
        
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
                var facingDirection = exitDirection switch
                {
                    ExitDirection.Left => Vector2.left,
                    ExitDirection.Right => Vector2.right,
                    ExitDirection.Up => Vector2.up,
                    ExitDirection.Down => Vector2.down,
                    _ => Vector2.zero
                };

                if(!_canExit) return false;
                playerController.RevealPlayer(revealPosition.position, facingDirection);
                ContainsPlayer = false;
                _spriteRenderer.sprite = emptyLockerSprite;
            }
            else if(!playerController.isHiding)
            {
                playerController.HidePlayer(gameObject, hidePosition.position);
                ContainsPlayer = true;
                if(canTrigger) OnInteractEvents?.Invoke();
                _spriteRenderer.sprite = hideLockerSprite;
            }
            else
            {
                return false;
            }
            
            GameManager.Instance.SoundSystem.Play(interactSound, transform);
            return true;
        }
        
        public void SetCanTrigger(bool value)
        {
            canTrigger = value;
        }

        public void CanExit(bool value)
        {
            _canExit = value;
        }        
        
        public bool CanInteract()
        {
            return true;
        }
    }
}
