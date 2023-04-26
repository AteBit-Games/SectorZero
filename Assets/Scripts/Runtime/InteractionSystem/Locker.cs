/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.Managers;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.InteractionSystem
{
    public class Locker : MonoBehaviour, IInteractable
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;

        [SerializeField] private Transform lockerPosition;
        [SerializeField] private Transform revealPosition;
        
        [SerializeField] private Sprite emptyLocker;
        [SerializeField] private Sprite playerLocker;
        
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
                _spriteRenderer.sprite = emptyLocker;
            }
            else
            {
                GameManager.Instance.PlayerController.HidePlayer(lockerPosition.position);
                _spriteRenderer.sprite = playerLocker;
            }
            
            return true;
        }
    }
}
