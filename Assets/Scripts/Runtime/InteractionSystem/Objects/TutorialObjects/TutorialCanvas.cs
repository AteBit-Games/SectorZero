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
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.InteractionSystem.Objects.TutorialObjects
{
    public class TutorialCanvas : MonoBehaviour, IInteractable, IPersistant
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private UnityEvent onTriggerEvents;
        [SerializeField] private Transform playerStandPosition;
        [SerializeField] private Item paintingSupplies;
        [SerializeField] private float delayBeforeTriggeringStage5 = 1f;
        
        [SerializeField] private List<UnityEvent> finishInteractEvents;
        [SerializeField] private UnityEvent onInteractFailedEvents;
        [SerializeField] public string persistentID;

        //----- Private Variables -----//
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private bool _hasInteracted;
        private PlayerController _playerController;
        private bool _failedToInteract;

        private static readonly int Paint = Animator.StringToHash("paint");
        private static readonly int Finish = Animator.StringToHash("finished");

        //========================= Unity Events =========================//
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _playerController = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);
        }
        
        //========================= Interface events =========================//

        public bool OnInteract(GameObject player)
        {
            onTriggerEvents.Invoke();
            GameManager.Instance.SoundSystem.Play(interactSound, transform.GetComponent<AudioSource>());

            //Setup player and trigger animation
            _animator.SetTrigger(Paint);
            player.transform.parent.position = playerStandPosition.position;
            _playerController.DisableInput();
            _playerController.SetFacingDirection(new Vector2(0f, 1f));
            
            //Disable collider and remove item from inventory
            GetComponent<Collider2D>().enabled = false;
            GameManager.Instance.InventorySystem.PlayerInventory.UseItemInInventory(paintingSupplies);

            return true;
        }
        
        public void OnInteractFailed(GameObject player)
        {
            if (!_failedToInteract)
            {
                _failedToInteract = true;
                onInteractFailedEvents.Invoke();
            }
        }
        
        public bool CanInteract()
        {
            return GameManager.Instance.InventorySystem.PlayerInventory.ContainsKeyItem(paintingSupplies) && !_hasInteracted;
        }
        
        //========================= Public Methods =========================//

        public void FinishInteraction()
        {
            _playerController.EnableInput();
            _hasInteracted = true;
            StartCoroutine(TriggerStage5());
            GameManager.Instance.SaveSystem.SaveGame();
        }
        
        //========================= Save System =========================//

        public void LoadData(SaveGame game)
        {
            if (game.tutorialData.canvas.ContainsKey(persistentID))
            {
                if (game.tutorialData.canvas[persistentID])
                {
                    StartCoroutine(TriggerStage5());
                    _animator.SetTrigger(Finish);
                    GetComponent<Collider2D>().enabled = false;
                }
            }
        }
        
        public void SaveData(SaveGame game)
        {
            game.tutorialData.canvas[persistentID] = _hasInteracted;
        }
        
        //========================= Coroutines =========================//
        
        private IEnumerator TriggerStage5()
        {
            yield return new WaitForSeconds(1f);
            finishInteractEvents[0].Invoke();
            yield return new WaitForSeconds(delayBeforeTriggeringStage5);
            finishInteractEvents[1].Invoke();
        }
    }
}
