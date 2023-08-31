/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using UnityEngine;

namespace Runtime.Misc.Triggers
{
    public class TriggerDelegate : MonoBehaviour
    {
        public Action triggerEvent;
        
        public void Trigger()
        {
            triggerEvent?.Invoke();
        }
    }
}
