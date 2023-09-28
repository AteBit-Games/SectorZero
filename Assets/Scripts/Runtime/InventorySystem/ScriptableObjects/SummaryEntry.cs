/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using UnityEngine;

namespace Runtime.InventorySystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Summary Entry", menuName = "Inventory System/Summary Entry")]
    public class SummaryEntry : ScriptableObject
    {
        public string itemRef;
        [TextArea] public string summaryContent;
        public bool isCompleted;
    }
}
