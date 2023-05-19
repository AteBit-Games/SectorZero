/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using UnityEngine;

namespace Runtime.InteractionSystem.Interfaces
{
    public interface IHideable
    {
        public bool ContainsPlayer { get; set; }
        public Transform MonsterInspectPosition { get; }
    }
}
