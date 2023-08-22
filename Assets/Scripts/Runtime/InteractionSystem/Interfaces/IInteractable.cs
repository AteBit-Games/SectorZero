/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.SoundSystem;
using UnityEngine;

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
