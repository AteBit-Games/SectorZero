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
using Runtime.AI;
using Runtime.SaveSystem;
using Runtime.SoundSystem;
using Runtime.UI;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Runtime.Managers
{
    [DefaultExecutionOrder(2)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [SerializeField] private Sound menuClickSound;
        [SerializeField] private Sound menuHoverSound;
        [SerializeField] public bool isMainMenu;
        
        [SerializeField] public bool testMode;
        public bool TestMode
        {
            get => testMode;
            set => testMode = value;
        }
        //========================= Interface =========================
        
        [HideInInspector] public InputReader inputReader;
        
        public DialogueManager DialogueSystem { get; private set;  }
        public InventoryManager InventorySystem { get; private set; }
        public SoundManager SoundSystem { get; private set; }
        public SaveManager SaveSystem { get; private set; }
        public NotificationManager NotificationManager { get; private set; }
        public AIManager AIManager { get; set; }

        public HUD HUD { get; private set; }
        private DeathScreen DeathScreen { get; set; }
        private PauseMenu PauseMenu { get; set; }
        private LoadingScreen LoadingScreen { get; set; }
        private EndScreen EndScreen { get; set; }
        
        private CinemachineVirtualCamera _camera;
        [HideInInspector] public Window activeWindow;
        private SceneInstance _sceneInstance;
        
        //====== Discord ========
        private const long ApplicationId = 1109955823121215509;
        private const string LargeImage = "sectorzerologo";
        private const string LargeText = "Sector Zero";
        [HideInInspector] public string details = "";
        private long _time;
        private Discord.Discord _discord;
        
        //========================= Unity Events =========================
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            Addressables.LoadAssetAsync<InputReader>("InputReader").Completed += handle =>
            {
                inputReader = handle.Result;
                inputReader.PauseEvent += HandlePause;
                inputReader.OpenInventoryEvent += OpenInventoryWindow;
                inputReader.CloseUIEvent += HandleEscape;
            };
            
            DontDestroyOnLoad(gameObject);
            
            Instance.LoadingScreen = GetComponentInChildren<LoadingScreen>();
            Instance.SoundSystem = GetComponent<SoundManager>();
            Instance.SaveSystem = GetComponent<SaveManager>();
            Instance.AIManager = GetComponent<AIManager>();
            GetComponent<AIManager>();

            InstantiateDiscord();
        }

        private void InstantiateDiscord()
        {
            try
            {
                _discord = new Discord.Discord(ApplicationId, (UInt64)CreateFlags.NoRequireDiscord);
                _time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void Update()
        {
            if(_discord == null) return;
            try
            {
                _discord.RunCallbacks();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void LateUpdate()
        {
            if(_discord == null) return;
            UpdateStatus();
        }

        private void OnApplicationQuit()
        {
            _discord?.Dispose();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            inputReader.PauseEvent -= HandlePause;
            inputReader.OpenInventoryEvent -= OpenInventoryWindow;
            inputReader.CloseUIEvent -= HandleEscape;
        }
        
        //========================= Public Methods =========================

        public Sound ClickSound()
        {
            return menuClickSound;
        }
        
        public Sound HoverSound()
        {
            return menuHoverSound;
        }
        
        public void HandleEscape()
        {
            if (isMainMenu)
            {
                var mainMenu = FindFirstObjectByType<MainMenu>(FindObjectsInactive.Include);

                if (mainMenu.isSettingsOpen)
                {
                    mainMenu.CloseSettings();
                    SoundSystem.Play(menuClickSound);
                    inputReader.SetUI();
                }
                else if(mainMenu.isSavesWindowOpen)
                {
                    mainMenu.CloseSavesMenu();
                    SoundSystem.Play(menuClickSound);
                    inputReader.SetUI();
                }
                else
                {
                    mainMenu.CloseAllPopups();
                }
                
                return;
            }
            
            if (activeWindow != null)
            {
                if(activeWindow.isSubWindowOpen) activeWindow.CloseSubWindow();
                else activeWindow.CloseWindow();
            }
        }

        public void ResetInput()
        {
            inputReader.SetGameplay();
        }
        
        public void DisableInput()
        {
            inputReader.SetUI();    
        }
        
        public void LoadScene(int sceneIndex)
        {
            TestMode = false;
            isMainMenu = sceneIndex == 0;
            activeWindow = null;
            
            SoundSystem.ResetSystem();
            AudioListener.pause = true;

            if (sceneIndex != 2)
            {
                StartCoroutine(LoadSceneAsync(sceneIndex));
            }
            else
            {
                //unload scene
                if(_sceneInstance.Scene.isLoaded) Addressables.UnloadSceneAsync(_sceneInstance);
                StartCoroutine(LoadMainSceneAsync());
            }
        }

        public void GameOver(DeathType deathType)
        {
            inputReader.SetUI();
            DeathScreen.Show(EnumUtils.GetDeathMessage(deathType));
            activeWindow = DeathScreen;

            var transposer = _camera.GetCinemachineComponent<CinemachineTransposer>();
            transposer.TweenValueFloat(4f, 0.35f, value =>
            {
                transposer.m_FollowOffset = new Vector3(-value, 0f, -10f);
            }).SetFrom(0f).SetEaseSineInOut();
        }
        
        public void EndGame()
        {
            Time.timeScale = 0f;
            inputReader.SetUI();
            EndScreen.OpenWindow();
            activeWindow = EndScreen;
        }
        
        //========================= Private Methods =========================
        
        private void OpenInventoryWindow()
        {
            if(isMainMenu || SceneManager.GetActiveScene().name == "Tutorial") return;
            switch (InventorySystem.isInventoryOpen)
            {
                case false:
                    InventorySystem.OpenWindow();
                    activeWindow = InventorySystem;
                    break;
                case true:
                    InventorySystem.CloseWindow();
                    break;
            }
        }

        private void HandlePause()
        {
            if(activeWindow != null) return;
            activeWindow = PauseMenu;
            PauseMenu.OpenWindow();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var volume = FindFirstObjectByType<Volume>();
            var vignette = volume.sharedProfile.components[0] as Vignette;
            if (vignette != null) vignette.intensity.value = 0f;
            var aberration = volume.sharedProfile.components[1] as ChromaticAberration;
            if (aberration != null) aberration.intensity.value = 0f;

            //Required objects for all scenes
            _camera = FindFirstObjectByType<CinemachineVirtualCamera>();
            NotificationManager = FindFirstObjectByType<NotificationManager>(FindObjectsInactive.Include);

            if (isMainMenu)
            {
                inputReader.SetUI();
                return;
            }

            //Required objects for gameplay scenes
            DialogueSystem = FindFirstObjectByType<DialogueManager>(FindObjectsInactive.Include);
            InventorySystem = FindFirstObjectByType<InventoryManager>(FindObjectsInactive.Include);
            PauseMenu = FindFirstObjectByType<PauseMenu>(FindObjectsInactive.Include);
            HUD = FindFirstObjectByType<HUD>(FindObjectsInactive.Include);
            DeathScreen = FindFirstObjectByType<DeathScreen>(FindObjectsInactive.Include);
            EndScreen = FindFirstObjectByType<EndScreen>(FindObjectsInactive.Include);
        }

        private void UpdateStatus()
        {
            try
            {
                var activityManager = _discord.GetActivityManager();
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
                        Start = _time
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
        
        //========================= Coroutines =========================
        
        private IEnumerator LoadSceneAsync(int sceneIndex)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
            LoadingScreen.ShowLoading();
            while (!asyncOperation.isDone)
            {
                yield return null;
            }
            
            if(sceneIndex == 0) LoadingScreen.HideLoading();
        }
        
        private IEnumerator LoadMainSceneAsync()
        {
            var asyncOperation = Addressables.LoadSceneAsync("SectorTwo", LoadSceneMode.Single, false);
            LoadingScreen.ShowLoading();
            while (!asyncOperation.IsDone)
            {
                yield return asyncOperation;
            }
            
            if (asyncOperation.Status == AsyncOperationStatus.Succeeded)
                yield return asyncOperation.Result.ActivateAsync();
            
            _sceneInstance = asyncOperation.Result;
        }

        public void FinishedLoading()
        {
            ResetInput();
            AudioListener.pause = false;
            SoundSystem.StartSounds();
            
            LoadingScreen.HideLoading();
            Time.timeScale = 1f;
        }
    }
}