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
    public class DeathScreen : Window
    {
        [HideInInspector] public bool isSavesWindowOpen;

        // Main Pause Items
        private VisualElement _deathWindow;
        private VisualElement _deathContainer;
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
        private VisualElement _savesWindow;
        
        private VisualElement _confirmDesktopPopup;
        private Button _confirmDesktopQuit;
        private Button _cancelDesktopQuit;

        private VisualElement _confirmMenuPopup;
        private Button _confirmMenuQuit;
        private Button _cancelMenuQuit;
        
        private SaveMenu _saveMenu;
        
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            _deathContainer = rootVisualElement.Q<VisualElement>("death-container");
            
            _feedbackForm = FindFirstObjectByType<FeedbackForm>();
            _savesWindow = rootVisualElement.Q<VisualElement>("saves-window");
            _saveMenu = GetComponent<SaveMenu>();
            
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
                OpenSavesMenu();
            });
            _loadButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Continue from a previous save point";
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            SetupPopups();
        }

        public void Show(string deathDescription)
        {
            _deathDescription.text = deathDescription;
            OpenWindow();
        }

        public override void OpenWindow()
        {
            UIUtils.ShowUIElement(_deathWindow);
        }
        
        public override void CloseWindow() {}
        
        public override void CloseSubWindow()
        {
            if (isSavesWindowOpen) CloseSavesMenu();
        }

        private void OpenSavesMenu()
        {
            UIUtils.HideUIElement(_deathContainer);
            UIUtils.ShowUIElement(_savesWindow);
            
            GameManager.Instance.SaveSystem.GetSaveGames();
            _saveMenu.ShowSaves();
            isSavesWindowOpen = true;
            isSubWindowOpen = true;
        }

        private void CloseSavesMenu()
        {
            UIUtils.HideUIElement(_savesWindow);
            UIUtils.ShowUIElement(_deathContainer);
            
            isSavesWindowOpen = false;
            isSubWindowOpen = false;
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
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            _confirmMenuQuit.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                GoToMainMenu();
            });
            _confirmMenuQuit.RegisterCallback<MouseEnterEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            _cancelMenuQuit.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                UIUtils.HideUIElement(_confirmMenuPopup);
            });
            _cancelMenuQuit.RegisterCallback<MouseEnterEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            //Bind Desktop Popup Buttons
            _quitDesktopButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                OpenConfirmPopup(_confirmDesktopPopup);
            });
            _quitDesktopButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Quit the game";
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
                UIUtils.HideUIElement(_confirmDesktopPopup);
            });
            _cancelDesktopQuit.RegisterCallback<MouseEnterEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
        }
        
        private static void OpenConfirmPopup(VisualElement popup)
        {
            UIUtils.ShowUIElement(popup);
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