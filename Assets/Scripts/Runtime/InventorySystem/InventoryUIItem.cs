/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.InventorySystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Runtime.InventorySystem
{
    public class InventoryUIItem 
    {
        [HideInInspector] public readonly BaseItem item;
        [HideInInspector] public VisualElement itemUI;
        
        public InventoryUIItem(BaseItem inventoryItem, VisualElement uiRef)
        {
            itemUI = uiRef;
            var itemImage = uiRef.Q<VisualElement>("item-image");
            
            if (inventoryItem != null)
            {
                itemImage.style.backgroundImage = inventoryItem.itemSprite.texture;
                item = inventoryItem;
            }
            else
            {
                itemImage.style.backgroundImage = null;
            }
        }

        public BaseItem OnClick()
        {
            return item;
        }
    }
}