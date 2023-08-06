/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/   
using System;
using System.Collections;
using Cinemachine;
using Runtime.DialogueSystem;
using Runtime.InventorySystem;
using Runtime.InputSystem;
using Discord;
using ElRaccoone.Tweens;
using Runtime.SaveSystem;
using Runtime.SoundSystem;
using Runtime.SoundSystem.ScriptableObjects;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Runtime.Managers
{
    [DefaultExecutionOrder(0)]
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
        [SerializeField] public bool testMode;

        public DialogueManager DialogueSystem { get; private set;  }
        public InventoryManager InventorySystem { get; private set; }
        public SoundManager SoundSystem { get; private set; }
        public AmbienceManager AmbienceManager { get; private set; }
        public SaveManager SaveSystem { get; private set; }
        public NotificationManager NotificationManager { get; private set; }
        public HUD HUD { get; private set; }
        
        private DeathScreen DeathScreen { get; set; }
        private PauseMenu PauseMenu { get; set; }
        private LoadingScreen LoadingScreen { get; set; }
        private EndScreen EndScreen { get; set; }
        
        private CinemachineVirtualCamera _camera;
        private bool _isReady;
        
        //====== Discord ========
        private const long ApplicationId = 1109955823121215509;
        private const string LargeImage = "sectorzerologo";
        private const string LargeText = "Sector Zero";
        [HideInInspector] public string details = "";
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
            
            DontDestroyOnLoad(gameObject);
            discord = new Discord.Discord(ApplicationId, (ulong)CreateFlags.NoRequireDiscord);
            time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            
            Instance.LoadingScreen = GetComponentInChildren<LoadingScreen>();
            Instance.SoundSystem = GetComponent<SoundManager>();
            Instance.AmbienceManager = GetComponentInChildren<AmbienceManager>();
            Instance.SaveSystem = GetComponent<SaveManager>();
            
            if(testMode)
            {
                SaveSystem.NewGame(true, SceneManager.GetActiveScene().buildIndex);
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
            inputReader.ContinueAction += FinishLoad;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            inputReader.PauseEvent -= HandlePause;
            inputReader.OpenInventoryEvent -= OpenInventoryWindow;
            inputReader.CloseUIEvent -= CloseInventoryWindow;
            inputReader.CloseUIEvent -= HandleEscape;
            inputReader.ContinueAction -= FinishLoad;
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
            if(isMainMenu || DeathScreen.isOpen) return;
            PauseMenu.Pause();
        }
        
        public void HandleEscape()
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
            var volume = FindObjectOfType<Volume>();
            var vignette = volume.sharedProfile.components[0] as Vignette;
            if (vignette != null) vignette.intensity.value = 0f;

            //Required objects for all scenes
            _camera = FindObjectOfType<CinemachineVirtualCamera>();
            NotificationManager = FindObjectOfType<NotificationManager>(true);
            SoundSystem.ResetSystem();

            if (isMainMenu)
            {
                inputReader.SetUI();
                return;
            }

            //Required objects for gameplay scenes
            DialogueSystem = FindObjectOfType<DialogueManager>(true);
            InventorySystem = FindObjectOfType<InventoryManager>(true);
            PauseMenu = FindObjectOfType<PauseMenu>(true);
            HUD = FindObjectOfType<HUD>(true);
            DeathScreen = FindObjectOfType<DeathScreen>(true);
            EndScreen = FindObjectOfType<EndScreen>(true);
            
            
            if(testMode)
            {
                ResetInput();
                Time.timeScale = 1f;
            }
            else
            {
                Time.timeScale = 0f;
            }
            SoundSystem.PauseAll();
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
                        LargeImage = LargeImage,
                        LargeText = LargeText
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

        public void GameOver(DeathType deathType)
        {
            inputReader.SetUI();
            DeathScreen.Show(EnumUtils.GetDeathMessage(deathType));
            
            var transposer = _camera.GetCinemachineComponent<CinemachineTransposer>();
            transposer.TweenValueFloat(4f, 0.35f, value =>
            {
                transposer.m_FollowOffset = new Vector3(-value, 0f, -10f);
            }).SetFrom(0f).SetEaseSineInOut();
        }
        
        public void LoadScene(int sceneIndex)
        {
            _isReady = false;
            inputReader.SetUI();
            StartCoroutine(LoadSceneAsync(sceneIndex));
        }
        
        private IEnumerator LoadSceneAsync(int sceneIndex)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
            LoadingScreen.ShowLoading();
            while (!asyncOperation.isDone)
            {
                yield return null;
            }
            LoadingScreen.ShowContinue();
            _isReady = true;
        }

        private void FinishLoad()
        {
            if (_isReady && LoadingScreen.isOpen)
            {
                Time.timeScale = 1f;
                ResetInput();
                LoadingScreen.HideLoading();
            }
        }
        
        public void EndGame()
        {
            Time.timeScale = 0f;
            inputReader.SetUI();
            EndScreen.Show();
        }
    }
}