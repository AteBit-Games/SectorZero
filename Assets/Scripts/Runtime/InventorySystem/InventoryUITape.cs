/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.DialogueSystem;
using Runtime.InventorySystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Runtime.InventorySystem
{
    public class InventoryUITape : InventoryUIItem
    {
        [HideInInspector] public Dialogue dialogue;
        
        public InventoryUITape(BaseItem inventoryItem, VisualElement uiRef, Dialogue dialogue) : base(inventoryItem, uiRef)
        {
            this.dialogue = dialogue;
        }
    }
}