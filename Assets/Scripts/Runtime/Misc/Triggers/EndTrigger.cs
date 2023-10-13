/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using Runtime.Managers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Runtime.Misc.Triggers
{
    public class EndTrigger : MonoBehaviour
    {
        public void Trigger()
        {
            GameManager.Instance.EndGame();
        }
    }
}
