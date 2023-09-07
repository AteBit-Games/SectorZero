/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using Runtime.Managers;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.UI
{
    public class EndScreen : Window
    {
        // Main Pause Items
        private VisualElement _endWindow;
        private UIDocument _uiDocument;
        
        //Buttons
        private Label _buttonDescription;
        private Button _quitMenuButton;
        private Button _quitDesktopButton;
        
        private VisualElement _confirmDesktopPopup;
        private Button _confirmDesktopQuit;
        private Button _cancelDesktopQuit;

        private VisualElement _confirmMenuPopup;
        private Button _confirmMenuQuit;
        private Button _cancelMenuQuit;
        
        private VisualElement _activePopup;
        
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;

            // Main Items
            _buttonDescription = rootVisualElement.Q<Label>("button-description");
            _buttonDescription.text = "Exit to main menu";
           
            _endWindow = rootVisualElement.Q<VisualElement>("end-window");
            
            _quitMenuButton = rootVisualElement.Q<Button>("quit-menu");
            _quitDesktopButton = rootVisualElement.Q<Button>("quit-desktop");
                
            _confirmDesktopPopup = rootVisualElement.Q<VisualElement>("confirm-desktop-popup");
            _confirmDesktopQuit = rootVisualElement.Q<Button>("confirm-desktop-option");
            _cancelDesktopQuit = rootVisualElement.Q<Button>("cancel-desktop-option");

            _confirmMenuPopup = rootVisualElement.Q<VisualElement>("confirm-menu-popup");
            _confirmMenuQuit = rootVisualElement.Q<Button>("confirm-menu-option");
            _cancelMenuQuit = rootVisualElement.Q<Button>("cancel-menu-option");

            SetupPopups();
        }

        public override void OpenWindow()
        {
            UIUtils.ShowUIElement(_endWindow);
        }
        
        public override void CloseWindow() {}
        
        public override void CloseSubWindow()
        {
            if(_activePopup != null) ClosePopup();
            GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
        }

        private void SetupPopups()
        {
            //Bind Menu Popup Buttons
            _quitMenuButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                UIUtils.HideUIElement(_confirmDesktopPopup);
                OpenConfirmPopup(_confirmMenuPopup);
            });
            _quitMenuButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Quit to main menu";
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            _confirmMenuQuit.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                GoToMainMenu();
            });
            
            _cancelMenuQuit.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                UIUtils.HideUIElement(_confirmMenuPopup);
            });
            
            //Bind Desktop Popup Buttons
            _quitDesktopButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                UIUtils.HideUIElement(_confirmMenuPopup);
                OpenConfirmPopup(_confirmDesktopPopup);
            });
            _quitDesktopButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Quit to desktop";
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            _confirmDesktopQuit.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                Application.Quit();
            });
            
            _cancelDesktopQuit.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                UIUtils.HideUIElement(_confirmDesktopPopup);
            });
        }
        
        private void OpenConfirmPopup(VisualElement popup)
        {
            ClosePopup();
            UIUtils.ShowUIElement(popup);
            
            _activePopup = popup;
            isSubWindowOpen = true;
        }
        
        private void ClosePopup()
        {
            if(_activePopup == null) return;
            
            UIUtils.HideUIElement(_activePopup);
            _activePopup = null;
            isSubWindowOpen = false;
        }

        private static void GoToMainMenu()
        {
            Time.timeScale = 1;
            GameManager.Instance.SoundSystem.StopAll();
            GameManager.Instance.isMainMenu = true;
            GameManager.Instance.ResetInput();
            GameManager.Instance.LoadScene(0);
        }
    }
}