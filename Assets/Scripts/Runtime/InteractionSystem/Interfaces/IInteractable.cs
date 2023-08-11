/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.InteractionSystem.Interfaces
{
    public interface IInteractable
    {
        [Tooltip("Sound to play when the player interacts with the object")] public Sound InteractSound { get; }
        public bool OnInteract(GameObject player);
        public void OnInteractFailed(GameObject player);
        public bool CanInteract();
    }
}
