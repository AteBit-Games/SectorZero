/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.DialogueSystem;
using Runtime.InventorySystem;
using Runtime.InputSystem;
using Runtime.SaveSystem;
using Runtime.SoundSystem;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        #region Header ASSET REFERENCES
        [Space(10)]
        [Header("ASSET REFERENCES")]
        #endregion
        [SerializeField] private InputReader inputReader;
        [SerializeField] public bool isMainMenu;
        
        
        #region Header DEBUG
        [Space(10)]
        [Header("DEBUG")]
        #endregion 
        [SerializeField] private bool testMode;

        public DialogueManager DialogueSystem { get; private set;  }
        public InventoryManager InventorySystem { get; private set; }
        public SoundManager SoundSystem { get; private set; }
        public SaveManager SaveSystem { get; private set; }
        private PauseMenu PauseMenu { get; set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        
            Instance = this;
            Instance.SoundSystem = GetComponent<SoundManager>();
            Instance.SaveSystem = GetComponent<SaveManager>();
        
            DontDestroyOnLoad(gameObject);
        
            if(testMode)
            {
                SaveSystem.NewGame();
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            inputReader.PauseEvent += HandlePause;
            inputReader.ResumeEvent += HandleResume;
            inputReader.OpenInventoryEvent += OpenInventoryWindow;
            inputReader.CloseInventoryEvent += CloseInventoryWindow;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            inputReader.PauseEvent -= HandlePause;
            inputReader.ResumeEvent -= HandleResume;
            inputReader.OpenInventoryEvent -= OpenInventoryWindow;
            inputReader.CloseInventoryEvent -= CloseInventoryWindow;
        }

        private void CloseInventoryWindow()
        {
            if(isMainMenu) return;

            Time.timeScale = 1;
            InventorySystem.CloseInventory();
        }

        private void OpenInventoryWindow()
        {
            if(isMainMenu) return;

            Time.timeScale = 0;
            InventorySystem.OpenInventory();
        }

        private void HandlePause()
        {
            if(isMainMenu) return;
            PauseMenu.Pause();
        }
        
        public void HandleResume()
        {
            if(isMainMenu) return;
            inputReader.SetGameplay();
            PauseMenu.Resume();
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if(isMainMenu) return;
            
            DialogueSystem = FindObjectOfType<DialogueManager>(true);
            InventorySystem = FindObjectOfType<InventoryManager>(true);
            PauseMenu = FindObjectOfType<PauseMenu>(true);
        }
        
        public void PlayGame(Sound sound)
        {
            ClickSound(sound);
            isMainMenu = false;
        }
        
        public void GoToMainMenu(Sound sound)
        {
            HandleResume();
            SoundSystem.StopAll();
            ClickSound(sound);
            isMainMenu = true;
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }

        public void QuitGame(Sound sound)
        {
            ClickSound(sound);
            Application.Quit();
        }
        
        public void ClickSound(Sound sound)
        {
            SoundSystem.Play(sound);
        }
    }
}