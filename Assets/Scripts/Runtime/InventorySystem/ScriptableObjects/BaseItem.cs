/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Runtime.InventorySystem.ScriptableObjects
{
    public class BaseItem : ScriptableObject
    {
        [Header("ITEM DETAILS")]
        public AssetReference itemRef;
        public string itemName;
        public Sprite itemSprite;
        [TextArea] public string itemDescription;
    }
}
