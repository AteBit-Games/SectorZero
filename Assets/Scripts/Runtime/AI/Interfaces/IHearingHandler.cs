/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using Runtime.SaveSystem;
using Runtime.SoundSystem;

namespace Runtime.AI.Interfaces
{
    public interface IHearingHandler
    {
        void OnHearing(NoiseEmitter sender);
    }
}
