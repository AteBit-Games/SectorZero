/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Runtime.Managers;
using Runtime.SaveSystem.Data;
using Runtime.Utils;
using UnityEngine.SceneManagement;

namespace Runtime.SaveSystem
{
    [Serializable]
    internal class PlayerConfig
    {
        public int sceneIndex;
        public Vector3 startPosition;
    }
    
    [DefaultExecutionOrder(-1)]
    public class SaveManager : MonoBehaviour
    {
        [Header("SAVE SYSTEM CONFIG")]
        [SerializeField] private string saveFileExtension;
        [SerializeField] private bool useEncryption;
        [SerializeField] private List<PlayerConfig> playerConfig;
        [HideInInspector] public bool saveExists;

        private List<IPersistant> _persistantObjects;
        private FileHandler _dataHandler;
        private Dictionary<Texture2D, SaveGame> _saveGames = new();
        private PlayerData _playerData;
        private SaveGame _activeSave;
        public SaveGame ActiveSave => _activeSave;

        // ======================================= UNITY METHODS =======================================
        
        private void Awake() 
        {
            var saveGamePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
            saveGamePath += "/My Games/Sector Zero/";
            _dataHandler = new FileHandler(saveGamePath, saveFileExtension, useEncryption ? EncryptionType.AES : EncryptionType.None);
            _playerData = _dataHandler.LoadPlayerData();
        }

        private void Start()
        {
            _saveGames = _dataHandler.LoadSaves();
            saveExists = _saveGames.Count > 0;
        }

        private void OnEnable() 
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable() 
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if(GameManager.Instance.isMainMenu) return;
            _persistantObjects = FindAllPersistenceObjects();

            if (!GameManager.Instance.TestMode && _activeSave != null) LoadData(_activeSave);
        }
        
        // ======================================= PUBLIC METHODS =======================================
        
        public void ContinueGame()
        {
            _activeSave = _saveGames.Values.First();
            GameManager.Instance.LoadScene(_activeSave.currentScene);
        }
        
        public void LoadGame(long saveTime)
        {
            _activeSave = _saveGames.Values.First(s => s.saveTime == saveTime);
            GameManager.Instance.LoadScene(_activeSave.currentScene);
        }
        
        public void NewGame(int scene)
        {
            _dataHandler.ClearSaves();
            _activeSave = new SaveGame();
            saveExists = false;
            
            SetNellieState(scene);
        }

        public void SaveGame()
        {
            if (_saveGames.Count == 3) _dataHandler.DeleteSave(_saveGames.Values.Last().saveTime);
 
            _activeSave ??= new SaveGame();
            SaveGame saveGame = new()
            {
                saveTime = DateTime.Now.Ticks,
                saveName = SceneManager.GetActiveScene().name,
                currentScene = SceneManager.GetActiveScene().buildIndex
            };
                        
            foreach (var persistentObject in _persistantObjects)
            {
                persistentObject.SaveData(saveGame);
            }

            _dataHandler.Save(saveGame);
        }

        public void SetNellieState(int sceneIndex)
        {
            var index = playerConfig.FindIndex(config => config.sceneIndex == sceneIndex);
            _activeSave.playerData.position = index == -1 ? new Vector3(0, 0, 0) : playerConfig[index].startPosition;
            _activeSave.playerData.enabled = false;
        }
        
        public Dictionary<Texture2D, SaveGame> GetSaveGames()
        {
            _saveGames = _dataHandler.LoadSaves();
            return _saveGames;
        }
        
        public void DeleteSave(long saveTime)
        {
            _dataHandler.DeleteSave(saveTime);
        }
        
        // ======================================= PRIVATE METHODS =======================================
        
        private void LoadData(SaveGame saveGame)
        {
            foreach (var persistantObject in _persistantObjects)
            {
                persistantObject.LoadData(saveGame);
            }
            
            StartCoroutine(FinishedLoading());
        }

        private static List<IPersistant> FindAllPersistenceObjects()
        {
            var dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IPersistant>();
            return new List<IPersistant>(dataPersistenceObjects);
        }
        
        //======================================== Player Data ========================================
        
        public void UpdatePlayerVSync(bool active)
        {
            _playerData.vSync = active;
            _dataHandler.SavePlayerData(_playerData);
        }
        
        public void UpdatePlayerDisplayMode(int mode)
        {
            _playerData.displayMode = mode;
            _dataHandler.SavePlayerData(_playerData);
        }
        
        public void UpdatePlayerMasterVolume(float volume)
        {
            _playerData.masterVolume = volume;
            _dataHandler.SavePlayerData(_playerData);
        }
        
        public void UpdatePlayerSoundsVolume(float volume)
        {
            _playerData.sfxVolume = volume;
            _dataHandler.SavePlayerData(_playerData);
        }
        
        public void UpdatePlayerMusicVolume(float volume)
        {
            _playerData.musicVolume = volume;
            _dataHandler.SavePlayerData(_playerData);
        }
        
        public PlayerData GetPlayerData()
        {
            return _playerData;
        }
        
        // ============================ COROUTINES ============================
        
        private IEnumerator FinishedLoading()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            GameManager.Instance.FinishedLoading();
        }
    }
}