/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

namespace Runtime.InteractionSystem
{
    public interface IPowered
    {
        bool IsPowered { get; set; }

        public void PowerOn();

        public void PowerOff();
    }
}