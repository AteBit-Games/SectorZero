/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using UnityEngine.UIElements;

namespace Runtime.Utils
{
    public static class UIUtils
    {
        public static void ShowUIElement(VisualElement element)
        {
            element.style.display = DisplayStyle.Flex;
            element.pickingMode = PickingMode.Position;
        }
        
        public static void HideUIElement(VisualElement element)
        {
            element.style.display = DisplayStyle.None;
            element.pickingMode = PickingMode.Ignore;
        }
    }
}
