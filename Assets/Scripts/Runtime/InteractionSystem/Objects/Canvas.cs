/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections;
using System.Collections.Generic;
using Runtime.InteractionSystem.Interfaces;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.Player;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.InteractionSystem.Objects
{
    public class Canvas : MonoBehaviour, IInteractable
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private Transform playerStandPosition;
        [SerializeField] private Item paintingSupplies;
        [SerializeField] private List<UnityEvent> finishTriggerEvents;
        [SerializeField] private float delayBeforeTriggeringStage5 = 1f;
        
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
            GameManager.Instance.SoundSystem.Play(interactSound, transform);

            player = player.transform.parent.gameObject;
            _playerController = player.GetComponent<PlayerController>();


            //Setup player and trigger animation
            _animator.SetTrigger(Paint);
            player.transform.position = playerStandPosition.position;
            _playerController.DisableInput();
            _playerController.LookAt(new Vector2(0f, 1f));
            
            //Disable collider and remove item from inventory
            GetComponent<Collider2D>().enabled = false;
            GameManager.Instance.InventorySystem.PlayerInventory.UseItemInInventory(paintingSupplies);
            _hasInteracted = true;

            return true;
        }

        public void FinishInteraction()
        {
            _playerController.EnableInput();
            StartCoroutine(TriggerStage5());
        }

        private IEnumerator TriggerStage5()
        {
            yield return new WaitForSeconds(1f);
            finishTriggerEvents[0].Invoke();
            yield return new WaitForSeconds(delayBeforeTriggeringStage5);
            finishTriggerEvents[1].Invoke();
        }
        
        public bool CanInteract()
        {
            return GameManager.Instance.InventorySystem.PlayerInventory.ContainsKeyItem(paintingSupplies) && !_hasInteracted;
        }
    }
}
