/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.Player;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Runtime.InteractionSystem.Objects
{
    internal enum ExitDirection
    {
        Left,
        Right,
        Up,
        Down
    }
    
    public class Locker : MonoBehaviour, IInteractable, IHideable
    {
        [FormerlySerializedAs("cooldown")] [SerializeField] private float cooldownAmount = 0.25f;
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private Transform inspectPosition;
        public Transform InspectPosition => inspectPosition;
        
        [SerializeField, Tooltip("The location to move the player when they enter the locker")] private Transform hidePosition;
        [SerializeField, Tooltip("The location to move the player when the exit the locker")] private Transform revealPosition;
        [SerializeField] private ExitDirection exitDirection;
        
        [SerializeField] private Sprite emptyLockerSprite;
        [SerializeField] private Sprite hideLockerSprite;
        
        [SerializeField] private UnityEvent onTriggerEvents;
        [SerializeField] private bool canTrigger;
        [SerializeField] private bool canExit = true;
        
        //----- Interface Properties -----//
        public bool ContainsPlayer { get; set; }

        //----- Private Variables -----//
        private SpriteRenderer _spriteRenderer;
        private bool _cooldownActive;

        //========================= Unity events =========================//
        
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            if(!canExit || _cooldownActive) return false;
            
            var playerController = player.GetComponentInParent<PlayerController>();

            if (playerController.isHiding && ContainsPlayer)
            {
                if(playerController.InputDisabled) return false;
                
                var facingDirection = exitDirection switch
                {
                    ExitDirection.Left => Vector2.left,
                    ExitDirection.Right => Vector2.right,
                    ExitDirection.Up => Vector2.up,
                    ExitDirection.Down => Vector2.down,
                    _ => Vector2.zero
                };

                playerController.RevealPlayer(revealPosition.position, facingDirection);
                ContainsPlayer = false;
                _spriteRenderer.sprite = emptyLockerSprite;
            }
            else if(!playerController.isHiding)
            {
                playerController.HidePlayer(gameObject, hidePosition.position);
                ContainsPlayer = true;
                if(canTrigger) onTriggerEvents?.Invoke();
                _spriteRenderer.sprite = hideLockerSprite;
            }
            else
            {
                return false;
            }
            
            _cooldownActive = true;
            StartCoroutine(Cooldown());
            
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

        //========================= Public Methods =========================//
        
        public void SetCanTrigger(bool value)
        {
            canTrigger = value;
        }
        
        public void SetCanExit(bool value)
        {
            canExit = value;
        }
        
        private IEnumerator Cooldown()
        {
            yield return new WaitForSeconds(cooldownAmount);
            _cooldownActive = false;
        }
    }
}
