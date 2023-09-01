/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using UnityEngine.UIElements;

namespace Runtime.InventorySystem
{
    public class InventoryUIItem 
    {
        public readonly BaseItem item;
        public readonly VisualElement itemUI;
        
        public InventoryUIItem(BaseItem inventoryItem, VisualElement uiRef)
        {
            itemUI = uiRef;
            var itemImage = uiRef.Q<VisualElement>("item-image");
            
            if (inventoryItem != null)
            {
                itemUI.AddToClassList("inventory-slot");
                itemImage.style.backgroundImage = inventoryItem.itemSprite.texture;
                item = inventoryItem;
            }
            else
            {
                itemUI.AddToClassList("empty-slot");
                itemImage.style.backgroundImage = null;
            }
        }

        public BaseItem OnClick()
        {
            GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
            itemUI.RemoveFromClassList("inventory-slot");
            itemUI.AddToClassList("selected-slot");
            return item;
        }

        public void Deselect()
        {
            itemUI.AddToClassList("inventory-slot");
            itemUI.RemoveFromClassList("selected-slot");
        }
    }
}