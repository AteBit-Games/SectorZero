/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.UI
{
    [DefaultExecutionOrder(5)]
    public class NotificationManager : MonoBehaviour
    {
        [SerializeField] private Sprite[] savingIconStates;
        [SerializeField] private float iconSpeed = 0.5f;
        [SerializeField] private Sound scribbleSound;
        private int _savingIndex;
        
        private UIDocument _uiDocument;

        // Pickup Notification Items
        private VisualElement _pickupContainer;
        private Label _pickupText;
        private VisualElement _pickupIcon;
        
        //result notification items
        private VisualElement _resultContainer;
        private Label _resultText;
        private VisualElement _resultIcon;
        
        //Saving notification items
        private VisualElement _savingIcon;
        
        //Misc
        private VisualElement _summaryContainer;
        private VisualElement _cheatAddedContainer;

        private Coroutine _savingLoopCoroutine;
        private Coroutine _pickupCoroutine;
        private Coroutine _resultCoroutine;
        private Coroutine _summaryCoroutine;
        private Coroutine _cheatCoroutine;
        
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
        
            // Pickup Notification Items
            _pickupIcon = rootVisualElement.Q<VisualElement>("pickup-image");
            _pickupText = rootVisualElement.Q<Label>("pickup-name");
            _pickupContainer = rootVisualElement.Q<VisualElement>("pickup-popup");
            
            _resultIcon = rootVisualElement.Q<VisualElement>("status-image");
            _resultText = rootVisualElement.Q<Label>("status-message");
            _resultContainer = rootVisualElement.Q<VisualElement>("report-status");
            
            _savingIcon = rootVisualElement.Q<VisualElement>("saving-icon");
            _summaryContainer = rootVisualElement.Q<VisualElement>("todo-popup");
            _cheatAddedContainer = rootVisualElement.Q<VisualElement>("cheat-popup");
        }
        
        public void ShowPickupNotification(BaseItem item)
        {
            _pickupIcon.style.backgroundImage = new StyleBackground(item.itemSprite);
            _pickupText.text = "Picked up "+item.itemName;
            _pickupContainer.AddToClassList("popup-show");

            if (_pickupCoroutine != null)
            {
                StopCoroutine(_pickupCoroutine);
                _pickupContainer.RemoveFromClassList("popup-show");
            }

            _pickupCoroutine = StartCoroutine(HidePickupNotification());
        }        
        private IEnumerator HidePickupNotification()
        { 
            yield return new WaitForSecondsRealtime(3.5f);
            _pickupContainer.RemoveFromClassList("popup-show");
            _pickupCoroutine = null;
        }
        
        public void ShowResultNotification(string result, Sprite resultImage)
        {
            _resultText.text = result;
            _resultIcon.style.backgroundImage = new StyleBackground(resultImage);

            if (_resultCoroutine != null)
            {
                StopCoroutine(_resultCoroutine);
                _resultContainer.RemoveFromClassList("result-show");
            }
            
            _resultContainer.AddToClassList("result-show");
            _resultCoroutine = StartCoroutine(HideResultNotification());
        }
        
        private IEnumerator HideResultNotification()
        {
            yield return new WaitForSecondsRealtime(3.5f);
            _resultContainer.RemoveFromClassList("result-show");
        }
        
        public void ShowSaving()
        {
            _savingIcon.AddToClassList("saving-show");
            StartCoroutine(HideLoading());
            _savingLoopCoroutine = StartCoroutine(SavingIcon());
        }
        
        public IEnumerator HideLoading()
        {
            yield return new WaitForSecondsRealtime(3.1f);
            _savingIcon.RemoveFromClassList("saving-show");
            if(_savingLoopCoroutine != null) StopCoroutine(_savingLoopCoroutine);
        }
        
        private IEnumerator SavingIcon()
        {
            _savingIcon.style.backgroundImage = new StyleBackground(savingIconStates[_savingIndex]);
            _savingIndex = (_savingIndex + 1) % savingIconStates.Length;
            yield return new WaitForSecondsRealtime(iconSpeed);
            _savingLoopCoroutine = StartCoroutine(SavingIcon());
        }
        
        public void ShowSummaryAddedNotification()
        {
            _summaryContainer.AddToClassList("todo-show");
            GameManager.Instance.SoundSystem.Play(scribbleSound);

            if (_pickupCoroutine != null)
            {
                StopCoroutine(_pickupCoroutine);
                _summaryContainer.RemoveFromClassList("todo-show");
            }

            _summaryCoroutine = StartCoroutine(HideSummaryAddedNotification());
        }        
        
        private IEnumerator HideSummaryAddedNotification()
        { 
            yield return new WaitForSecondsRealtime(3.5f);
            _summaryContainer.RemoveFromClassList("todo-show");
            _summaryCoroutine = null;
        }
        
        public void ShowCheatEnabledNotification()
        {
            _cheatAddedContainer.AddToClassList("cheat-popup-show");
            
            if (_pickupCoroutine != null)
            {
                StopCoroutine(_pickupCoroutine);
                _cheatAddedContainer.RemoveFromClassList("cheat-popup-show");
            }

            _cheatCoroutine = StartCoroutine(HideCheatEnabledNotification());
        }        
        
        private IEnumerator HideCheatEnabledNotification()
        { 
            yield return new WaitForSecondsRealtime(3.5f);
            _cheatAddedContainer.RemoveFromClassList("cheat-popup-show");
            _cheatCoroutine = null;
        }
    }
}
