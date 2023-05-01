/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

namespace Runtime.AI.Interfaces
{
    public interface IHearingHandler
    {
        float LowerHearingThreshold { get; }
        void OnHearing(NoiseEmitter sender, float intensity);
    }
}
