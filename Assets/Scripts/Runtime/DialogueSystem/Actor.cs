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
        [SerializeField] private new string name;
        [SerializeField] private Sprite sprite;
    
        public string Name => name;
        public Sprite Sprite => sprite;
    }
}
