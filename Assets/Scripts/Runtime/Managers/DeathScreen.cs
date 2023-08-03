﻿/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.ReporterSystem;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Runtime.Managers
{
    public class DeathScreen : MonoBehaviour
    {
        [HideInInspector] public bool isOpen;

        // Main Pause Items
        private VisualElement _deathWindow;
        private UIDocument _uiDocument;
        
        //Buttons
        private Label _buttonDescription;
        private Label _deathDescription;
        private Button _loadButton;
        private Button _quitMenuButton;
        private Button _quitDesktopButton;
        private Button _reportBugButton;
        
        // Other Items
        private FeedbackForm _feedbackForm;
        
        private VisualElement _confirmDesktopPopup;
        private Button _confirmDesktopQuit;
        private Button _cancelDesktopQuit;

        private VisualElement _confirmMenuPopup;
        private Button _confirmMenuQuit;
        private Button _cancelMenuQuit;
        
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            _feedbackForm = FindObjectOfType<FeedbackForm>();
            
            // Main Items
            _buttonDescription = rootVisualElement.Q<Label>("button-description");
            _buttonDescription.text = "Load from the last checkpoint";
            
            _deathDescription = rootVisualElement.Q<Label>("death-description");
            _deathWindow = rootVisualElement.Q<VisualElement>("death-window");
            
            _quitMenuButton = rootVisualElement.Q<Button>("quit-menu");
            _quitDesktopButton = rootVisualElement.Q<Button>("quit-desktop");
                
            _confirmDesktopPopup = rootVisualElement.Q<VisualElement>("confirm-desktop-popup");
            _confirmDesktopQuit = rootVisualElement.Q<Button>("confirm-desktop-option");
            _cancelDesktopQuit = rootVisualElement.Q<Button>("cancel-desktop-option");

            _confirmMenuPopup = rootVisualElement.Q<VisualElement>("confirm-menu-popup");
            _confirmMenuQuit = rootVisualElement.Q<Button>("confirm-menu-option");
            _cancelMenuQuit = rootVisualElement.Q<Button>("cancel-menu-option");
            
            _reportBugButton = rootVisualElement.Q<Button>("report-problem");
            _reportBugButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                _feedbackForm.ShowForm();
            });
            _reportBugButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Report a problem";
            });

            _loadButton = rootVisualElement.Q<Button>("load");
            _loadButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                LoadGame();
            });
            _loadButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Load from the last checkpoint";
            });
            
            SetupPopups();
        }

        public void Show(string deathDescription)
        {
            _deathDescription.text = deathDescription;
            isOpen = true;
            UI_Utils.ShowUIElement(_deathWindow);
        }
        
        private void LoadGame()
        {
            GameManager.Instance.SaveSystem.LoadGame();
            _feedbackForm.HideForm();
        }
        
        private void SetupPopups()
        {
            //Bind Menu Popup Buttons
            _quitMenuButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                OpenConfirmPopup(_confirmMenuPopup);
            });
            _quitMenuButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Quit to main menu";
            });
            
            _confirmMenuQuit.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                GoToMainMenu();
            });
            
            _cancelMenuQuit.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                UI_Utils.HideUIElement(_confirmMenuPopup);
            });
            
            //Bind Desktop Popup Buttons
            _quitDesktopButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                OpenConfirmPopup(_confirmDesktopPopup);
            });
            _quitDesktopButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Quit to desktop";
            });
            
            _confirmDesktopQuit.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                Application.Quit();
            });
            
            _cancelDesktopQuit.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                UI_Utils.HideUIElement(_confirmDesktopPopup);
            });
        }
        
        private static void OpenConfirmPopup(VisualElement popup)
        {
            UI_Utils.ShowUIElement(popup);
        }

        public void GoToMainMenu()
        {
            Time.timeScale = 1;
            GameManager.Instance.SoundSystem.StopAll();
            GameManager.Instance.isMainMenu = true;
            GameManager.Instance.ResetInput();
            SceneManager.LoadScene(0);
        }
    }
}