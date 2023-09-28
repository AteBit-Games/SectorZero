/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using UnityEngine;

namespace Runtime.InventorySystem.ScriptableObjects
{
    public class BaseItem : ScriptableObject
    {
        public string itemRef;
        public string itemName;
        public Sprite itemSprite;
        [TextArea] public string itemDescription;
    }
}
