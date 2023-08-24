/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/


using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.UI
{
    public class HUD : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private VisualElement _throwableContainer;
        private VisualElement _throwableIcon;
        
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            
            _throwableContainer = rootVisualElement.Q<VisualElement>("active-throwable");
            _throwableIcon = rootVisualElement.Q<VisualElement>("throwable-image");
        }
        
        public void SetThrowableIcon(Sprite throwableSprite)
        {
            _throwableIcon.style.backgroundImage = new StyleBackground(throwableSprite);
            ShowThrowableIcon(true);
        }
        
        public void ShowThrowableIcon(bool show)
        {
            _throwableContainer.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
