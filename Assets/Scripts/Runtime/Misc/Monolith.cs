/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using Runtime.Managers;
using UnityEngine;

namespace Runtime.Misc
{
    [DefaultExecutionOrder(6)]
    public class Monolith : MonoBehaviour
    {
        public void TriggerEnd()
        {
            GameManager.Instance.EndGame();
        }
    }
}
