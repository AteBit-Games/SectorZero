﻿/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using Runtime.Managers;
using Runtime.ReporterSystem;
using Runtime.SaveSystem;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.UI
{
    [DefaultExecutionOrder(5)]
    public class MainMenu : MonoBehaviour
    {
        [HideInInspector] public bool isSettingsOpen;
        [HideInInspector] public bool isSavesWindowOpen;

        // Main Pause Items
        private Label _buttonDescription;
        private Button _continueButton;
        private Button _loadButton;
        private Button _newGameButton;
        private Button _settingsButton;
        private Button _quitButton;
        private VisualElement _mainMenuWindow;
        private UIDocument _uiDocument;
        private Button _reportBugButton;
        
        // Misc Items
        private VisualElement _settingsWindow;
        private VisualElement _savesWindow;
        private FeedbackForm _feedbackForm;
        private Button _discordButton;
        
        // Popups
        private VisualElement _confirmNewGamePopup;
        private Button _confirmNewGame;
        private Button _cancelNewGame;
        
        private VisualElement _confirmTutorialPopup;
        private Button _yesTutorial;
        private Button _noTutorial;
        
        private VisualElement _confirmQuitPopup;
        private Button _confirmDesktopQuit;
        private Button _cancelDesktopQuit;

        private SaveMenu _saveMenu;

        private void Awake()
        {
            GameManager.Instance.SaveSystem.RegisterSaves();
            
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            _feedbackForm = FindFirstObjectByType<FeedbackForm>();
            
            _mainMenuWindow = rootVisualElement.Q<VisualElement>("main-menu-window");            
            _settingsWindow = rootVisualElement.Q<VisualElement>("settings-window");
            
            _savesWindow = rootVisualElement.Q<VisualElement>("saves-window");
            _saveMenu = GetComponent<SaveMenu>();
            
            _confirmNewGamePopup = rootVisualElement.Q<VisualElement>("confirm-new-game-popup");
            _confirmNewGame = rootVisualElement.Q<Button>("confirm-new-game-option");  
            _cancelNewGame = rootVisualElement.Q<Button>("cancel-new-game-option");

            _confirmTutorialPopup = rootVisualElement.Q<VisualElement>("confirm-tutorial-popup");
            _yesTutorial = rootVisualElement.Q<Button>("confirm-tutorial-option");
            _noTutorial = rootVisualElement.Q<Button>("cancel-tutorial-option");
            
            _confirmQuitPopup = rootVisualElement.Q<VisualElement>("confirm-quit-popup");
            _confirmDesktopQuit = rootVisualElement.Q<Button>("confirm-desktop-option");
            _cancelDesktopQuit = rootVisualElement.Q<Button>("cancel-desktop-option");
            
            _buttonDescription = _mainMenuWindow.Q<Label>("button-description");
            
            var saveExists = GameManager.Instance.SaveSystem.saveExists;
            _buttonDescription.text = saveExists ? "Continue from the last save point" : "Start a new game";

            
            // Main Menu Items
            _continueButton = rootVisualElement.Q<Button>("continue");
            SetButtonState(_continueButton, saveExists);
            _continueButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                ContinueGame();
            });
            _continueButton.RegisterCallback<MouseEnterEvent>(_ => {
                if (saveExists)
                {
                    _buttonDescription.text = "Continue from the last save point";
                    GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
                }
            });
            
            _loadButton = rootVisualElement.Q<Button>("load-game");
            SetButtonState(_loadButton, saveExists);
            _loadButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound()); 
                OpenSavesMenu();
            });
            _loadButton.RegisterCallback<MouseEnterEvent>(_ => {
                if (saveExists)
                {
                    _buttonDescription.text = "Continue from a previous save point";
                    GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
                }
            });
            
            _reportBugButton = rootVisualElement.Q<Button>("report-problem");
            _reportBugButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                _feedbackForm.ShowForm();
                
                UIUtils.HideUIElement(_confirmQuitPopup);
                UIUtils.HideUIElement(_confirmTutorialPopup);
            });
            _reportBugButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Report a problem";
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            _discordButton = rootVisualElement.Q<Button>("discord");
            _discordButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                Application.OpenURL("https://discord.com/invite/TynJMyUCfY");
            });
            _discordButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Join Our Discord Server";
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });

            _newGameButton = rootVisualElement.Q<Button>("new-game");
            _newGameButton.RegisterCallback<ClickEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                OpenConfirmPopup(saveExists ? _confirmNewGamePopup : _confirmTutorialPopup);
            });
            _newGameButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Start a new game";
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            _settingsButton = rootVisualElement.Q<Button>("settings");
            _settingsButton.RegisterCallback<ClickEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                OpenSettings();
            });
            _settingsButton.RegisterCallback<MouseEnterEvent>(_ =>
            {

                _buttonDescription.text = "Change the game settings";
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            _quitButton = rootVisualElement.Q<Button>("quit");
            _quitButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                OpenConfirmPopup(_confirmQuitPopup);
            });
            _quitButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Quit the game";
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            SetupPopups();
            
        }
        
        private void SetupPopups()
        {
            _confirmNewGame.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                OpenConfirmPopup(_confirmTutorialPopup);
                UIUtils.HideUIElement(_confirmNewGamePopup);
            });
            _confirmNewGame.RegisterCallback<MouseEnterEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            _cancelNewGame.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                UIUtils.HideUIElement(_confirmNewGamePopup);
            });
            _cancelNewGame.RegisterCallback<MouseEnterEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            _yesTutorial.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                StartNewGame("Tutorial");
            });
            _yesTutorial.RegisterCallback<MouseEnterEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            _noTutorial.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                StartNewGame("SectorTwo");
            });
            _noTutorial.RegisterCallback<MouseEnterEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });

            _confirmDesktopQuit.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                Application.Quit();
            });
            _confirmDesktopQuit.RegisterCallback<MouseEnterEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            _cancelDesktopQuit.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                UIUtils.HideUIElement(_confirmQuitPopup);
            });
            _cancelDesktopQuit.RegisterCallback<MouseEnterEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
        }
        
        private static void OpenConfirmPopup(VisualElement popup)
        {
            UIUtils.ShowUIElement(popup);
        }

        private void OpenSettings()
        {
            UIUtils.HideUIElement(_mainMenuWindow);
            UIUtils.ShowUIElement(_settingsWindow);
            
            UIUtils.HideUIElement(_confirmQuitPopup);
            UIUtils.HideUIElement(_confirmTutorialPopup);
            
            isSettingsOpen = true;
        }
        
        public void CloseSettings()
        {
            UIUtils.HideUIElement(_settingsWindow);
            UIUtils.ShowUIElement(_mainMenuWindow);
            isSettingsOpen = false;
        }

        private static void StartNewGame(string levelName)
        {
            GameManager.Instance.SaveSystem.NewGame(levelName);
            GameManager.Instance.isMainMenu = false;
            GameManager.Instance.LoadScene(levelName);
        }
        
        private void OpenSavesMenu()
        {
            UIUtils.HideUIElement(_mainMenuWindow);
            UIUtils.ShowUIElement(_savesWindow);
            
            GameManager.Instance.SaveSystem.GetSaveGames();
            _saveMenu.ShowSaves();
            isSavesWindowOpen = true;
        }
        
        public void CloseSavesMenu()
        {
            UIUtils.HideUIElement(_savesWindow);
            UIUtils.ShowUIElement(_mainMenuWindow);
            isSavesWindowOpen = false;
        }
        
        public static void ContinueGame()
        {
            GameManager.Instance.isMainMenu = false;
            GameManager.Instance.SaveSystem.ContinueGame();
        }
        
        private static void SetButtonState(VisualElement button, bool state)
        {
            button.SetEnabled(state);
            button.style.opacity = state ? 1 : 0.5f;
            if(!state)
            {
                button.AddToClassList("button-disabled");
            }
            else
            {
                button.RemoveFromClassList("button-disabled");
            }
        }

        public void CloseAllPopups()
        {
            UIUtils.HideUIElement(_confirmQuitPopup);
            UIUtils.HideUIElement(_confirmTutorialPopup);
            UIUtils.HideUIElement(_confirmNewGamePopup);
        }
    }
}