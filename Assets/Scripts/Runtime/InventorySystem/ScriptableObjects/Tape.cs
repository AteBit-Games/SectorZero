/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.DialogueSystem;
using UnityEngine;

namespace Runtime.InventorySystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/Tape")]
    public class Tape : BaseItem
    {
        public Dialogue dialogue;
    }
}