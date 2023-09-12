/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using UnityEngine;

namespace Runtime.SoundSystem
{
    public interface ISoundEntity
    {
        public AudioSource AudioSource { get;}
        public Sound Sound { get; }
    }
}