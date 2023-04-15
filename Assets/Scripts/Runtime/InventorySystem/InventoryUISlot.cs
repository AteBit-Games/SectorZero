/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.InventorySystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.InventorySystem
{
    public class InventoryUISlot : MonoBehaviour
    {
        [HideInInspector] public BaseItem item;
        [SerializeField] private Image itemImage;
        
        public void InitializeItem(BaseItem inventoryItem)
        {
            item = inventoryItem;
            itemImage.sprite = inventoryItem.itemSprite;
        }
    }
}