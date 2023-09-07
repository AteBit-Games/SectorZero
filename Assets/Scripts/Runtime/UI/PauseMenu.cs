/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using Runtime.Managers;
using Runtime.ReporterSystem;
using Runtime.SaveSystem;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Runtime.UI
{
    public class PauseMenu : Window
    {
        [HideInInspector] public bool isSettingsOpen;
        [HideInInspector] public bool isSavesWindowOpen;
        
        // Main Pause Items
        private Label _buttonDescription;
        private Button _resumeButton;
        private Button _loadButton;
        private Button _settingsButton;
        private Button _quitButton;
        private VisualElement _pauseWindow;
        private UIDocument _uiDocument;
        private Button _reportBugButton;
        
        // Settings Items
        private VisualElement _settingsContainer;
        private VisualElement _settingsWindow;
        private VisualElement _overlay;
        private VisualElement _savesWindow;
        private FeedbackForm _feedbackForm;
        
        private VisualElement _confirmMenuPopup;
        private Button _confirmMenuQuit;
        private Button _cancelMenuQuit;
        
        private SaveMenu _saveMenu;
        
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            _feedbackForm = FindFirstObjectByType<FeedbackForm>();
            
            // Main Pause Items
            _buttonDescription = rootVisualElement.Q<Label>("button-description");
            _pauseWindow = rootVisualElement.Q<VisualElement>("pause-window");
            _settingsWindow = rootVisualElement.Q<VisualElement>("SettingsMenu");
            _settingsContainer = rootVisualElement.Q<VisualElement>("settings-window");
            
            _saveMenu = GetComponent<SaveMenu>();
            _savesWindow = rootVisualElement.Q<VisualElement>("saves-window");
            _overlay = rootVisualElement.Q<VisualElement>("overlay");
            
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

            // Main Pause Items
            _resumeButton = rootVisualElement.Q<Button>("resume");
            _resumeButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                UIUtils.HideUIElement(_confirmMenuPopup);
                CloseWindow();
            });
            _resumeButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Resume playing the game";
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });

            _loadButton = rootVisualElement.Q<Button>("load");
            _loadButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                UIUtils.HideUIElement(_confirmMenuPopup);
                OpenSavesMenu();
            });
            _loadButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Continue from a previous save point";
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            _settingsButton = rootVisualElement.Q<Button>("settings");
            _settingsButton.RegisterCallback<ClickEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                OpenSettings();
            });
            _settingsButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Change the game settings";
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            _quitButton = rootVisualElement.Q<Button>("quit");
            _quitButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                OpenConfirmPopup(_confirmMenuPopup);
            });
            _quitButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Quit to main menu";
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            SetupPopups();
        }

        public override void OpenWindow()
        {
            Time.timeScale = 0;
            UIUtils.ShowUIElement(_pauseWindow);
            UIUtils.ShowUIElement(_overlay);
        }
        
        public override void CloseWindow()
        {
            Time.timeScale = 1;
            GameManager.Instance.ResetInput();
            GameManager.Instance.SoundSystem.ResumeAll();
            GameManager.Instance.activeWindow = null;
            _feedbackForm.HideForm();
            
            UIUtils.HideUIElement(_pauseWindow);
            UIUtils.HideUIElement(_overlay);
            UIUtils.HideUIElement(_settingsContainer);
            UIUtils.HideUIElement(_settingsWindow);
            UIUtils.HideUIElement(_confirmMenuPopup);
        }
        
        public override void CloseSubWindow()
        {
            if(isSettingsOpen) CloseSettings();
            else if(isSavesWindowOpen) CloseSavesMenu();
            
            isSubWindowOpen = false;
        }
        
       private void OpenSavesMenu()
        {
            UIUtils.HideUIElement(_pauseWindow);
            UIUtils.ShowUIElement(_savesWindow);
            
            GameManager.Instance.SaveSystem.GetSaveGames();
            _saveMenu.ShowSaves();
            isSavesWindowOpen = true;
            isSubWindowOpen = true;
        }
        
        private void CloseSavesMenu()
        {
            UIUtils.HideUIElement(_savesWindow);
            UIUtils.ShowUIElement(_pauseWindow);
            
            isSavesWindowOpen = false;
        }

        private void OpenSettings()
        {
            UIUtils.HideUIElement(_pauseWindow);
            UIUtils.ShowUIElement(_settingsWindow);
            UIUtils.ShowUIElement(_settingsContainer);
            
            isSettingsOpen = true;
            isSubWindowOpen = true;
            
            // Hide Elements
            _feedbackForm.HideForm();
            UIUtils.HideUIElement(_confirmMenuPopup);
        }
        
        private void CloseSettings()
        {
            UIUtils.HideUIElement(_settingsWindow);
            UIUtils.HideUIElement(_settingsContainer);
            UIUtils.ShowUIElement(_pauseWindow);
            
            isSettingsOpen = false;
        }
        
        public void GoToMainMenu()
        {
            Time.timeScale = 1;
            GameManager.Instance.SoundSystem.StopAll();
            GameManager.Instance.isMainMenu = true;
            GameManager.Instance.ResetInput();
            GameManager.Instance.LoadScene(0);
        }
        
        private static void OpenConfirmPopup(VisualElement popup)
        {
            UIUtils.ShowUIElement(popup);
        }
        
        private void SetupPopups()
        {
            //Bind Menu Popup Buttons
            _confirmMenuQuit.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                GoToMainMenu();
            });
            _confirmMenuPopup.RegisterCallback<MouseEnterEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            _cancelMenuQuit.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                UIUtils.HideUIElement(_confirmMenuPopup);
            });
            _cancelMenuQuit.RegisterCallback<MouseEnterEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
        }
    }
}