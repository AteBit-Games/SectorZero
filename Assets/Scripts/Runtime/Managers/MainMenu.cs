﻿/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.ReporterSystem;
using Runtime.SoundSystem.ScriptableObjects;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Runtime.Managers
{
    public class MainMenu : MonoBehaviour
    {
        [HideInInspector] public bool isSettingsOpen;

        // Main Pause Items
        private Label _buttonDescription;
        private Button _continueButton;
        private Button _newGameButton;
        private Button _settingsButton;
        private Button _quitButton;
        private VisualElement _mainMenuWindow;
        private UIDocument _uiDocument;
        private Button _reportBugButton;
        
        // Settings Items
        private VisualElement _settingsWindow;
        private FeedbackForm _feedbackForm;
        
        private VisualElement _confirmTutorialPopup;
        private Button _yesTutorial;
        private Button _noTutorial;
        
        private VisualElement _confirmQuitPopup;
        private Button _confirmDesktopQuit;
        private Button _cancelDesktopQuit;
        

        private void Start()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            _feedbackForm = FindObjectOfType<FeedbackForm>();
            
            _buttonDescription = rootVisualElement.Q<Label>("button-description");
            _mainMenuWindow = rootVisualElement.Q<VisualElement>("main-menu-window");
            _settingsWindow = rootVisualElement.Q<VisualElement>("settings-window");

            _confirmTutorialPopup = rootVisualElement.Q<VisualElement>("confirm-tutorial-popup");
            _yesTutorial = rootVisualElement.Q<Button>("confirm-tutorial-option");
            _noTutorial = rootVisualElement.Q<Button>("cancel-tutorial-option");
            
            _confirmQuitPopup = rootVisualElement.Q<VisualElement>("confirm-quit-popup");
            _confirmDesktopQuit = rootVisualElement.Q<Button>("confirm-desktop-option");
            _cancelDesktopQuit = rootVisualElement.Q<Button>("cancel-desktop-option");
            
            // Main Menu Items
            _continueButton = rootVisualElement.Q<Button>("continue");
            SetButtonState(_continueButton, GameManager.Instance.SaveSystem.saveExists);
            _continueButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                LoadGame();
            });
            _continueButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Continue from the last checkpoint";
            });
            
            _reportBugButton = rootVisualElement.Q<Button>("report-problem");
            _reportBugButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                _feedbackForm.ShowForm();
            });
            _reportBugButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Report a problem";
            });

            _newGameButton = rootVisualElement.Q<Button>("new-game");
            _newGameButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                //OpenConfirmPopup(_confirmTutorialPopup);
                StartNewGame(1);
            });
            _newGameButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Start a new game";
            });
            
            _settingsButton = rootVisualElement.Q<Button>("settings");
            _settingsButton.RegisterCallback<ClickEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                OpenSettings();
            });
            _settingsButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Change the game settings";
            });
            
            _quitButton = rootVisualElement.Q<Button>("quit");
            _quitButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                OpenConfirmPopup(_confirmQuitPopup);
            });
            _quitButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Quit the game";
            });
            
            SetupPopups();
        }
        
        private void SetupPopups()
        {
            _yesTutorial.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                StartNewGame(1);
            });
            
            _noTutorial.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                StartNewGame(2);
            });

            _confirmDesktopQuit.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                Application.Quit();
            });
            
            _cancelDesktopQuit.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                UI_Utils.HideUIElement(_confirmQuitPopup);
            });
        }
        
        private static void OpenConfirmPopup(VisualElement popup)
        {
            UI_Utils.ShowUIElement(popup);
        }
        
        public void OpenSettings()
        {
            _mainMenuWindow.style.display = DisplayStyle.None;
            _settingsWindow.style.display = DisplayStyle.Flex;
            isSettingsOpen = true;
        }
        
        public void CloseSettings()
        {
            _mainMenuWindow.style.display = DisplayStyle.Flex;
            _settingsWindow.style.display = DisplayStyle.None;
            isSettingsOpen = false;
        }

        private static void StartNewGame(int level)
        {
            GameManager.Instance.SaveSystem.NewGame(false);
            GameManager.Instance.isMainMenu = false;
            GameManager.Instance.LoadScene(level);
        }
        
        public static void LoadGame()
        {
            GameManager.Instance.isMainMenu = false;
            GameManager.Instance.SaveSystem.LoadGame();
        }
        
        public void PlaySound(Sound sound)
        {
            GameManager.Instance.SoundSystem.Play(sound);
        }
        
        public static void QuitGame()
        {
            Application.Quit();
        }
        
        private static void SetButtonState(VisualElement button, bool state)
        {
            button.SetEnabled(state);
            button.style.opacity = state ? 1 : 0.5f;
        }
    }
}