/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/   
using System;
using Runtime.DialogueSystem;
using Runtime.InventorySystem;
using Runtime.InputSystem;
using Discord;
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
        [SerializeField] private Sound menuClickSound;
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
        public NotificationManager NotificationManager { get; set; }
       
        private PauseMenu PauseMenu { get; set; }
        
        
        //====== Discord ========
        private const long applicationId = 1109955823121215509;
        private const string largeImage = "sector_zero_icon";
        private const string largeText = "Sector Zero";
        public string details = "";
        private long time;
        private Discord.Discord discord;
        
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
            discord = new Discord.Discord(applicationId, (ulong)CreateFlags.NoRequireDiscord);
            time = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            if(testMode)
            {
                SaveSystem.NewGame();
            }
        }

        private void Update()
        {
            try
            {
                discord.RunCallbacks();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void LateUpdate()
        {
            UpdateStatus();
        }

        private void OnApplicationQuit()
        {
            discord.Dispose();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            inputReader.PauseEvent += HandlePause;
            inputReader.OpenInventoryEvent += OpenInventoryWindow;
            inputReader.CloseUIEvent += CloseInventoryWindow;
            inputReader.CloseUIEvent += HandleEscape;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            inputReader.PauseEvent -= HandlePause;
            inputReader.OpenInventoryEvent -= OpenInventoryWindow;
            inputReader.CloseUIEvent -= CloseInventoryWindow;
            inputReader.CloseUIEvent -= HandleEscape;
        }

        private void CloseInventoryWindow()
        {
            if(isMainMenu) return;
           //InventorySystem.CloseInventory();
        }

        private void OpenInventoryWindow()
        {
            if(isMainMenu) return;
            //InventorySystem.OpenInventory();
        }

        private void HandlePause()
        {
            if(isMainMenu) return;
            PauseMenu.Pause();
        }
        
        private void HandleEscape()
        {
            if (isMainMenu)
            {
                var mainMenu = FindObjectOfType<MainMenu>(true);
                if (mainMenu.isSettingsOpen)
                {
                    mainMenu.CloseSettings();
                    inputReader.SetUI();
                }
            }
            else if(PauseMenu.isPaused)
            {
                if(PauseMenu.isSettingsOpen)
                {
                    PauseMenu.CloseSettings();
                    inputReader.SetUI();
                    Time.timeScale = 0f;
                }
                else
                {
                    PauseMenu.Resume();
                }
            }
        }

        public void ResetInput()
        {
            inputReader.SetGameplay();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            NotificationManager = FindObjectOfType<NotificationManager>(true);

            if (isMainMenu)
            {
                inputReader.SetUI();
                return;
            }
            
            DialogueSystem = FindObjectOfType<DialogueManager>(true);
            InventorySystem = FindObjectOfType<InventoryManager>(true);
            PauseMenu = FindObjectOfType<PauseMenu>(true);
        }
        
        public Sound ClickSound()
        {
            return menuClickSound;
        }
        
        private void UpdateStatus()
        {
            try
            {
                var activityManager = discord.GetActivityManager();
                var activity = new Activity
                {
                    State = "",
                    Details = details,
                    Assets =
                    {
                        LargeImage = largeImage,
                        LargeText = largeText
                    },
                    Timestamps =
                    {
                        Start = time
                    }
                };

                activityManager.UpdateActivity(activity, _ =>
                {
                    // ignored
                });
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}