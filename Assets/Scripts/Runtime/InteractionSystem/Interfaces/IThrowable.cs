/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.SoundSystem;
using UnityEngine;


namespace Runtime.InteractionSystem.Interfaces
{
    public interface IThrowable
    {
        public Sound DropSound { get; }
        public void OnDrop(Transform dropPosition);
    }
}
