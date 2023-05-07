/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using UnityEngine;

namespace Runtime.DialogueSystem
{
    [CreateAssetMenu(fileName = "New Actor", menuName = "Dialogue System/Actor")]
    public class Actor : ScriptableObject
    {
        [SerializeField, Tooltip("Name of the actor")] private new string name;
        [SerializeField, Tooltip("Sprite to display in the UI")] private Sprite sprite;
    
        public string Name => name;
        public Sprite Sprite => sprite;
    }
}
