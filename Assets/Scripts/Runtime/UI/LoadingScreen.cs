/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
using Runtime.Managers;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.UI
{
    [DefaultExecutionOrder(5)]
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Sprite[] iconStates;
        [SerializeField] private float iconSpeed = 0.5f;

        private UIDocument _uiDocument;

        private VisualElement _loadContainer;
        private VisualElement _loadingIcon;
        
        private int _loadingIndex;
        private Coroutine _loadingIconCoroutine;
        
        private void Start()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            
            _loadContainer = rootVisualElement.Q<VisualElement>("load-overlay");
            _loadingIcon = rootVisualElement.Q<VisualElement>("loading-icon");
        }
        
        public void ShowLoading()
        {
            if(_loadingIconCoroutine != null) StopCoroutine(_loadingIconCoroutine);
            _loadingIconCoroutine = StartCoroutine(LoadingIcon());

            UIUtils.ShowUIElement(_loadContainer);
        }
        
        public void HideLoading()
        {
            if(_loadingIconCoroutine != null) StopCoroutine(_loadingIconCoroutine);
            UIUtils.HideUIElement(_loadContainer);
            _loadContainer.RemoveFromClassList("loading-complete");
        }
        
        private IEnumerator LoadingIcon()
        {
            _loadingIcon.style.backgroundImage = new StyleBackground(iconStates[_loadingIndex]);
            _loadingIndex = (_loadingIndex + 1) % iconStates.Length;
            yield return new WaitForSecondsRealtime(iconSpeed);
            _loadingIconCoroutine = StartCoroutine(LoadingIcon());
        }
    }
}
