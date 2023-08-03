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
    public class Canvas : MonoBehaviour, IInteractable
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private Transform playerStandPosition;
        
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private bool _hasInteracted;
        private PlayerController _playerController;
        
        private static readonly int Paint = Animator.StringToHash("paint");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public bool OnInteract(GameObject player)
        {
            player = player.transform.parent.gameObject;
            
            player.transform.position = playerStandPosition.position;
            _playerController = player.GetComponent<PlayerController>();
            _playerController.DisableInput();
            _playerController.LookAt(new Vector2(0f, 1f));

            _animator.SetTrigger(Paint);
            _hasInteracted = true;
            GameManager.Instance.SoundSystem.Play(interactSound, transform);
            return true;
        }

        public void FinishInteraction()
        {
            _playerController.EnableInput();
        }
        
        public bool CanInteract()
        {
            return !_hasInteracted;
        }
    }
}
