/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.ReporterSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Runtime.Managers
{
    public class PauseMenu : MonoBehaviour
    {
        [HideInInspector] public bool isSettingsOpen;
        [HideInInspector] public bool isPaused;
        
        // Main Pause Items
        private Label _buttonDescription;
        private Button _resumeButton;
        private Button _loadButton;
        private Button _settingsButton;
        private Button _quitButton;
        private VisualElement _pauseWindow;
        private VisualElement _pauseMenuContainer;
        private UIDocument _uiDocument;
        private Button _reportBugButton;
        
        // Settings Items
        private VisualElement _settingsContainer;
        private VisualElement _overlay;
        private FeedbackForm _feedbackForm;
        
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            _feedbackForm = FindObjectOfType<FeedbackForm>();
            
            // Main Pause Items
            _buttonDescription = rootVisualElement.Q<Label>("button-description");
            _pauseMenuContainer = rootVisualElement.Q<VisualElement>("pause-container");
            _pauseWindow = rootVisualElement.Q<VisualElement>("pause-window");
            _settingsContainer = rootVisualElement.Q<VisualElement>("settings-window");
            _overlay = rootVisualElement.Q<VisualElement>("overlay");

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
                Resume();
            });
            _resumeButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Resume playing the game";
            });

            _loadButton = rootVisualElement.Q<Button>("load");
            _loadButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                LoadGame();
            });
            _loadButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Load from the last checkpoint";
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
                GoToMainMenu();
            });
            _quitButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Quit the game";
            });
        }
        
        public void Pause()
        {
            Time.timeScale = 0; 
            _pauseWindow.style.display = DisplayStyle.Flex;
            _overlay.style.display = DisplayStyle.Flex;
            isPaused = true;
        }
        
        public void Resume()
        {
            Time.timeScale = 1;
            GameManager.Instance.ResetInput();
            GameManager.Instance.SoundSystem.ResumeAll();
            _pauseWindow.style.display = DisplayStyle.None;
            _overlay.style.display = DisplayStyle.None;
            isPaused = false;
            _feedbackForm.HideForm();
        }

        public void LoadGame()
        {
            GameManager.Instance.SaveSystem.LoadGame();
            _feedbackForm.HideForm();
        }
        
        public void OpenSettings()
        {
            _pauseMenuContainer.style.display = DisplayStyle.None;
            _settingsContainer.style.display = DisplayStyle.Flex;
            isSettingsOpen = true;
            _feedbackForm.HideForm();
        }
        
        public void CloseSettings()
        {
            _pauseMenuContainer.style.display = DisplayStyle.Flex;
            _settingsContainer.style.display = DisplayStyle.None;
            isSettingsOpen = false;
        }
        
        public void GoToMainMenu()
        {
            Time.timeScale = 1;
            GameManager.Instance.SoundSystem.StopAll();
            GameManager.Instance.isMainMenu = true;
            GameManager.Instance.ResetInput();
            SceneManager.LoadScene(0);
        }
        
        public static void QuitGame()
        {
            Application.Quit();
        }
    }
}