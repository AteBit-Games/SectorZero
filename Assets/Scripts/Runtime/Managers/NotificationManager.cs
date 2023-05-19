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
        
        
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
        
            // Pickup Notification Items
            _pickupIcon = rootVisualElement.Q<VisualElement>("pickup-image");
            _pickupText = rootVisualElement.Q<Label>("pickup-name");
            _pickupContainer = rootVisualElement.Q<VisualElement>("pickup-popup");
        }
        
        public void ShowPickupNotification(BaseItem item)
        {
            _pickupIcon.style.backgroundImage = new StyleBackground(item.itemSprite);
            _pickupText.text = "Picked up "+item.itemName;
            _pickupContainer.AddToClassList("popup-show");
            
            Invoke(nameof(HidePickupNotification), 3.5f);
        }
        
        private void HidePickupNotification()
        {
            _pickupContainer.RemoveFromClassList("popup-show");
        }
    }
}
