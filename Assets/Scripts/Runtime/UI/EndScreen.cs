/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
using Runtime.Managers;
using Runtime.Utils;
using Tweens;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.UI
{
    [DefaultExecutionOrder(5)]
    public class EndScreen : Window
    {
        private VisualElement _endWindow;
        private UIDocument _uiDocument;
        
        private VisualElement _developers;
        private VisualElement _playtesters;
        private VisualElement _thanks;
        
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            
            _endWindow = rootVisualElement.Q<VisualElement>("end-window");
            _developers = rootVisualElement.Q<VisualElement>("Developers");
            _playtesters = rootVisualElement.Q<VisualElement>("Playtesters");
            _thanks = rootVisualElement.Q<VisualElement>("Thanks");
        }

        public override void OpenWindow()
        {
            UIUtils.ShowUIElement(_endWindow);
            _developers.style.opacity = 0;
            _playtesters.style.opacity = 0;
            _thanks.style.opacity = 0;
            
            StartCoroutine(StartDevelopers());
        }

        private IEnumerator StartThanks()
        {
            yield return new WaitForSecondsRealtime(0.3f);
            
            var thanksTween = new FloatTween
            {
                to = 1f,
                from = 0f,
                duration = 1.2f,
                useUnscaledTime = true,
                onUpdate = (_, value) =>
                {
                    _thanks.style.opacity = value;
                }
            };
            gameObject.AddTween(thanksTween);

            yield return new WaitForSecondsRealtime(3f);
            
            var thanksFadeTween = new FloatTween
            {
                to = 0f,
                from = 1f,
                duration = 0.8f,
                useUnscaledTime = true,
                onUpdate = (_, value) =>
                {
                    _thanks.style.opacity = value;
                }
            };
            gameObject.AddTween(thanksFadeTween);
            
            yield return new WaitForSecondsRealtime(1f);
            CloseWindow();
        }
        
        private IEnumerator StartDevelopers()
        {
            yield return new WaitForSecondsRealtime(0.3f);
            
            var developersTween = new FloatTween
            {
                to = 1f,
                from = 0f,
                duration = 0.8f,
                useUnscaledTime = true,
                onUpdate = (_, value) =>
                {
                    _developers.style.opacity = value;
                }
            };
            gameObject.AddTween(developersTween);
            
            yield return new WaitForSecondsRealtime(8f);
            
            var developersFadeTween = new FloatTween
            {
                to = 0f,
                from = 1f,
                duration = 0.8f,
                useUnscaledTime = true,
                onUpdate = (_, value) =>
                {
                    _developers.style.opacity = value;
                }
            };
            gameObject.AddTween(developersFadeTween);
            yield return new WaitForSecondsRealtime(1f);
            StartCoroutine(StartPlaytesters());
        }

        private IEnumerator StartPlaytesters()
        {
            yield return new WaitForSecondsRealtime(0.3f);
            
            var playtestersTween = new FloatTween
            {
                to = 1f,
                from = 0f,
                duration = 0.8f,
                useUnscaledTime = true,
                onUpdate = (_, value) =>
                {
                    _playtesters.style.opacity = value;
                }
            };
            gameObject.AddTween(playtestersTween);
            
            yield return new WaitForSecondsRealtime(8f);
            
            var playtestersFadeTween = new FloatTween
            {
                to = 0f,
                from = 1f,
                duration = 0.8f,
                useUnscaledTime = true,
                onUpdate = (_, value) =>
                {
                    _playtesters.style.opacity = value;
                }
            };
            gameObject.AddTween(playtestersFadeTween);
            yield return new WaitForSecondsRealtime(2.5f);
            StartCoroutine(StartThanks());
        }

        public override void CloseWindow()
        {
            GameManager.Instance.LoadScene("MainMenu");
        }
        
        public override void CloseSubWindow(){ }
    }
}