/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using UnityEngine;

namespace Runtime.InventorySystem.ScriptableObjects
{
    public enum NoteType
    {
        Handwritten,
        Research,
    }
    
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/Note")]
    public class Note : BaseItem
    {
        public NoteType noteType;
        public string title;
        public string author;
        public string date;
        public string content;
    }
}