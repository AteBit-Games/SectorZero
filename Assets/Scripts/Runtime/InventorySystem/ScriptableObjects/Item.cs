/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using UnityEngine;

namespace Runtime.InventorySystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/Item")]
    public class Item : BaseItem
    {
        public string itemDescription;
    }
}