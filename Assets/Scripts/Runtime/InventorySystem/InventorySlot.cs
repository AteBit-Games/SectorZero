/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using UnityEngine.UIElements;

namespace Runtime.InventorySystem
{
    public class InventorySlot 
    {
        private readonly BaseItem _item;
        private readonly VisualElement _inventorySlot;
        
        public InventorySlot(BaseItem inventoryItem, VisualElement uiRef)
        {
            _inventorySlot = uiRef;
            _item = inventoryItem;
            
            var itemImage = uiRef.Q<VisualElement>("item-image");
            if (inventoryItem != null)
            {
                _inventorySlot.AddToClassList("inventory-slot");
                itemImage.style.backgroundImage = inventoryItem.itemSprite.texture;
            }
            else
            {
                _inventorySlot.AddToClassList("empty-slot");
                itemImage.style.backgroundImage = null;
            }
        }

        public BaseItem OnClick()
        {
            GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
            _inventorySlot.RemoveFromClassList("inventory-slot");
            _inventorySlot.AddToClassList("selected-slot");
            return _item;
        }

        public void Deselect()
        {
            _inventorySlot.AddToClassList("inventory-slot");
            _inventorySlot.RemoveFromClassList("selected-slot");
        }
    }
}