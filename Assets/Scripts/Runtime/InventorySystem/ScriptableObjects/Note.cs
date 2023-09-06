/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using UnityEngine;

namespace Runtime.InventorySystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/Note")]
    public class Note : BaseItem
    {
        public string title;
        public string content;
        public string footer;
    }
}