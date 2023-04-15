/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using UnityEngine;

namespace Runtime.InventorySystem.ScriptableObjects
{
    public class BaseItem : ScriptableObject
    {
        [Header("ITEM DETAILS")]
        public string itemName;
        public Sprite itemSprite;
        public ItemType itemType;
    }
}
