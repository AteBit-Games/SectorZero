/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/
using UnityEngine;

namespace Runtime.UI
{
    [DefaultExecutionOrder(5)]
    public abstract class Window : MonoBehaviour
    {
        [HideInInspector] public bool isSubWindowOpen;

        public abstract void OpenWindow();
        public abstract void CloseWindow();
        public abstract void CloseSubWindow();
    }
}