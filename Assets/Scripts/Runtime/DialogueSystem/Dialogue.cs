/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections.Generic;
using Runtime.InventorySystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.DialogueSystem
{
    [Serializable]
    public class DialogueLine
    {
        [SerializeField, Tooltip("The actor that will be speaking the line")] public Actor actor;
        [SerializeField, Tooltip("The line that the actor will be speaking"), TextArea(5, 15)] public string line;
    }
    
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField, Tooltip("The date of the dialogue")] public string date;
        [SerializeField, Tooltip("Lines that the actor will be speaking in order")] public List<DialogueLine> dialogueLines;
        [SerializeField, Tooltip("Can dialogue be skipped")] public bool canSkip;
        [SerializeField, Tooltip("Can dialogue be skipped")] public float autoSkipDelay;
        [SerializeField, Tooltip("If this dialogue should trigger an event")] public bool trigger;
        [SerializeField, Tooltip("If this dialogue should trigger an event")] public bool addSummaryEntry;
        [SerializeField, Tooltip("If this dialogue should trigger an event")] public List<SummaryEntry> summaryEntry;
    }
}
