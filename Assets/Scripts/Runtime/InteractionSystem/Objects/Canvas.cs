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
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.InteractionSystem.Objects
{
    public class Canvas : MonoBehaviour, IInteractable, IPersistant
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private UnityEvent onTriggerEvents;
        [SerializeField] private Transform playerStandPosition;
        [SerializeField] private Item paintingSupplies;
        [SerializeField] private List<UnityEvent> finishTriggerEvents;
        [SerializeField] private float delayBeforeTriggeringStage5 = 1f;
                
        [SerializeField] private string persistentID;
        public string ID
        {
            get => persistentID;
            set => persistentID = value;
        }
        
        [SerializeField] private UnityEvent onInteractFailedEvents;
        public UnityEvent OnInteractFailedEvents => onInteractFailedEvents;
        public bool failedToInteract { get; set; }


        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private bool _hasInteracted;
        private PlayerController _playerController;

        private static readonly int Paint = Animator.StringToHash("paint");
        private static readonly int Finish = Animator.StringToHash("finished");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _playerController = FindObjectOfType<PlayerController>(true);
        }

        public bool OnInteract(GameObject player)
        {
            onTriggerEvents.Invoke();
            GameManager.Instance.SoundSystem.Play(interactSound, transform);

            //Setup player and trigger animation
            _animator.SetTrigger(Paint);
            player.transform.parent.position = playerStandPosition.position;
            _playerController.DisableInput();
            _playerController.LookAt(new Vector2(0f, 1f));
            
            //Disable collider and remove item from inventory
            GetComponent<Collider2D>().enabled = false;
            GameManager.Instance.InventorySystem.PlayerInventory.UseItemInInventory(paintingSupplies);

            return true;
        }

        public void FinishInteraction()
        {
            _playerController.EnableInput();
            _hasInteracted = true;
            StartCoroutine(TriggerStage5());
            GameManager.Instance.SaveSystem.SaveGame();
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

        public UnityEvent OnInteractEvents { get; }

        public void LoadData(SaveData data)
        {
            if (data.tutorialData.canvas.ContainsKey(persistentID))
            {
                if (data.tutorialData.canvas[persistentID])
                {
                    StartCoroutine(TriggerStage5());
                    _animator.SetTrigger(Finish);
                    GetComponent<Collider2D>().enabled = false;
                }
            }
        }
        
        public void SaveData(SaveData data)
        {
            data.tutorialData.canvas[persistentID] = _hasInteracted;
        }
    }
}
