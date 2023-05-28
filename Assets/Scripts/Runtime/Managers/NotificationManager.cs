using System.Collections;
using Runtime.InventorySystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.Managers
{
    public class NotificationManager : MonoBehaviour
    {
        private UIDocument _uiDocument;

        // Pickup Notification Items
        private VisualElement _pickupContainer;
        private Label _pickupText;
        private VisualElement _pickupIcon;
        
        //result notification items
        private VisualElement _resultContainer;
        private Label _resultText;
        private VisualElement _resultIcon;
        
        private Coroutine _pickupCoroutine;
        private Coroutine _resultCoroutine;
        
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
    }
}
