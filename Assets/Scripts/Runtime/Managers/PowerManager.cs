/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections.Generic;
using Runtime.InteractionSystem.Objects.Powered;
using UnityEngine;

namespace Runtime.Managers
{
    [DefaultExecutionOrder(2)]
    public class PowerManager : MonoBehaviour
    {
        [SerializeField] private List<FuseBox> fuseBoxes;

        public void DisableAllBreakers()
        {
            foreach (var fusebox in fuseBoxes)
            {
                fusebox.SetPowered(false);
            }
        }
        
        public void EnableAllBreakers()
        {
            foreach (var fusebox in fuseBoxes)
            {
                fusebox.SetFuseTrue();
                fusebox.SetPowered(true);
            }
        }
    }
}