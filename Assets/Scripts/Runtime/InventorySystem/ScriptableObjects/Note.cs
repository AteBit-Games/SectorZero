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
        MonolithNews,
        MoonNews
    }
    
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/Note")]
    public class Note : BaseItem
    {
        public NoteType noteType;
        public string title;
        public string author;
        public string date;
        [TextArea(15,20)] public string content;
    }
}