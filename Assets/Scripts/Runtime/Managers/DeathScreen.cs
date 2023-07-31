/****************************************************************
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
        [HideInInspector] public bool isConfirmOpen;

        // Main Pause Items
        private VisualElement _deathWindow;
        private UIDocument _uiDocument;
        
        //Buttons
        private Label _buttonDescription;
        private Label _deathDescription;
        private Button _loadButton;
        private Button _quitButton;
        private Button _reportBugButton;
        
        // Other Items
        private FeedbackForm _feedbackForm;
        private VisualElement _confirmPopup;
        private Button _confirmDesktopQuit;
        private Button _confirmMenuQuit;
        
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            _feedbackForm = FindObjectOfType<FeedbackForm>();
            
            // Main Pause Items
            _buttonDescription = rootVisualElement.Q<Label>("button-description");
            _buttonDescription.text = "Load from the last checkpoint";
            
            _deathDescription = rootVisualElement.Q<Label>("death-description");
            _deathWindow = rootVisualElement.Q<VisualElement>("death-window");
            
            _confirmPopup = rootVisualElement.Q<VisualElement>("confirm-popup");
            _confirmDesktopQuit = rootVisualElement.Q<Button>("quit-game-option");
            _confirmDesktopQuit.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                Application.Quit();
            });
            _confirmMenuQuit = rootVisualElement.Q<Button>("quit-to-menu-option");
            _confirmMenuQuit.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                GoToMainMenu();
            });
            
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

            _quitButton = rootVisualElement.Q<Button>("quit");
            _quitButton.RegisterCallback<ClickEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                OpenConfirmPopup();
            });
            _quitButton.RegisterCallback<MouseEnterEvent>(_ => {
                _buttonDescription.text = "Quit the game";
            });
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
        
        public void OpenConfirmPopup()
        {
            isConfirmOpen = true;
            UI_Utils.ShowUIElement(_confirmPopup);
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