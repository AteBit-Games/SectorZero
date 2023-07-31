using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.Utils
{
    public static class UI_Utils
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
