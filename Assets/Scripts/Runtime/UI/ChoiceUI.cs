/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using System.Collections.Generic;
using Runtime.Managers;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Runtime.UI
{
    [Serializable]
    internal class Choice
    {
        public string choiceText;
        public UnityEvent choiceActions;
    }
    
    [DefaultExecutionOrder(5)]
    public class ChoiceUI : Window
    {
        [SerializeField] private List<Choice> choices;
        
        private UIDocument _uiDocument;
        private UIType _activeUIType;
        
        //Notes
        private VisualElement _choiceContainer;
        private Button _choiceButton1;
        private Button _choiceButton2;

        //=============================== Unity Events ===============================//
        
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            
            _choiceContainer = rootVisualElement.Q<VisualElement>("choice-view");
            _choiceButton1 = _choiceContainer.Q<Button>("choice-one");
            _choiceButton2 = _choiceContainer.Q<Button>("choice-two");
        }
        
        //=============================== Window Overrides ===============================//
        
        public override void OpenWindow()
        {
            UIUtils.ShowUIElement(_choiceContainer);
                        
            GameManager.Instance.SoundSystem.PauseAll();
            GameManager.Instance.DisableInput();
            
            Time.timeScale = 0;
            
            _choiceButton1.Q<Label>().text = choices[0].choiceText;
            _choiceButton1.RegisterCallback<ClickEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                GameManager.Instance.SaveSystem.SetNellieState("SectorThree", true);
                choices[0].choiceActions?.Invoke();
            });
            _choiceButton1.RegisterCallback<MouseEnterEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            _choiceButton2.Q<Label>().text = choices[1].choiceText;
            _choiceButton2.RegisterCallback<ClickEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                GameManager.Instance.SaveSystem.SetNellieState("SectorZero", true);
                choices[1].choiceActions?.Invoke();
            });
            _choiceButton2.RegisterCallback<MouseEnterEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
        }

        public override void CloseWindow()
        {
            Time.timeScale = 1;
            GameManager.Instance.activeWindow = null;
            UIUtils.HideUIElement(_choiceContainer);
            GameManager.Instance.ResetInput();
            GameManager.Instance.SoundSystem.ResumeAll();
        }

        public override void CloseSubWindow()
        {
            throw new NotImplementedException();
        }
    }
}
