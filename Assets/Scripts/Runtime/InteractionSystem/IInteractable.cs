/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem
{
    public interface IInteractable
    {
        public Sound InteractSound { get; }

        public bool OnInteract(GameObject player);
    }
}
