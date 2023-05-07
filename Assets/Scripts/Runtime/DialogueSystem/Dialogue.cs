/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.DialogueSystem
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField, Tooltip("Lines that the actor will be speaking in order")] public List<string> lines;
        [SerializeField, Tooltip("Reference to the actor that will be speaking the lines")] public Actor actor;
    }
}
