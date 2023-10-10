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
using UnityEngine.SceneManagement;

namespace Runtime.SaveSystem
{
    [Serializable]
    internal class PlayerConfig
    {
        public string levelName;
        public Vector3 startPosition;
    }
    
    [DefaultExecutionOrder(1)]
    public class SaveManager : MonoBehaviour
    {
        [Header("SAVE SYSTEM CONFIG")]
        [SerializeField] private string saveFileExtension;
        [SerializeField] private bool useEncryption;
        [SerializeField] private List<PlayerConfig> playerConfig;
        [HideInInspector] public bool saveExists;

        private List<IPersistant> _persistantObjects;
        private FileHandler _dataHandler;
        private PlayerData _playerData;
        private SaveGame _activeSave;
        
        private Dictionary<Texture2D, SaveGame> _saveGamesData = new();
        private List<SaveGame> _saveGames = new();

        // ======================================= UNITY METHODS =======================================
        
        private void Awake() 
        {
            //persistant data path
            var saveGamePath = Application.persistentDataPath;
            _dataHandler = new FileHandler(saveGamePath, saveFileExtension, useEncryption ? EncryptionType.AES : EncryptionType.None);
            _playerData = _dataHandler.LoadPlayerData();
        }

        private void Start()
        {
            RegisterSaves();
        }

        public void RegisterSaves()
        {
            _saveGamesData = _dataHandler.LoadSaves();
            _saveGames = _saveGamesData.Values.ToList();
            saveExists = _saveGamesData.Count > 0;
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
            _saveGamesData = _dataHandler.LoadSaves();
            _activeSave = _saveGamesData.Values.First();
            GameManager.Instance.LoadScene(_activeSave.levelName);
        }
        
        public void LoadGame(long saveTime)
        {
            _activeSave = _saveGamesData.Values.First(s => s.saveTime == saveTime);
            GameManager.Instance.LoadScene(_activeSave.levelName);
        }
        
        public void NewGame(string levelName)
        {
            _dataHandler.ClearSaves();
            _activeSave = new SaveGame();
            saveExists = false;
            
            GameManager.Instance.AIManager.StartNewGame();
            SetNellieState(levelName);
        }

        public void SaveGame()
        {
            if (_saveGames.Count >= 3) _dataHandler.DeleteSave(_saveGames.Last().saveTime);
            
            SaveGame saveGame = new()
            {
                saveTime = DateTime.Now.Ticks,
                levelName = SceneManager.GetActiveScene().name,
            };
                        
            foreach (var persistentObject in _persistantObjects)
            {
                persistentObject.SaveData(saveGame);
            }

            var newSave = _dataHandler.Save(saveGame);
            _saveGames.Add(newSave.saveGame);
            _activeSave = saveGame;
        }

        public void SetNellieState(string levelName)
        {
            var index = playerConfig.FindIndex(config => config.levelName == levelName);
            _activeSave.playerData.position = index == -1 ? new Vector3(0, 0, 0) : playerConfig[index].startPosition;
            _activeSave.playerData.enabled = false;
        }
        
        public Dictionary<Texture2D, SaveGame> GetSaveGames()
        {
            _saveGamesData = _dataHandler.LoadSaves();
            return _saveGamesData;
        }
        
        public void DeleteSave(long saveTime)
        {
            _dataHandler.DeleteSave(saveTime);
        }
        
        // ======================================= PRIVATE METHODS =======================================
        
        private void LoadData(SaveGame saveGame)
        {
            var ids = new Dictionary<IPersistant, string>();
            foreach (var persistantObject in _persistantObjects)
            {
                var id = persistantObject.LoadData(saveGame);
                if(id != "AIManager")
                {
                    if(ids.ContainsValue(id)) Debug.LogError($"Duplicate ID found: {id} in {ids.First(pair => pair.Value == id).Key} and {persistantObject}");
                    ids.Add(persistantObject, id);
                }
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
        
        public void UpdatePlayerBrightness(float brightnessGain)
        {
            _playerData.brightnessGain = brightnessGain;
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
        
        public void UpdatePlayerVoicesVolume(float volume)
        {
            _playerData.voicesVolume = volume;
            _dataHandler.SavePlayerData(_playerData);
        }
        
        public void UpdatePlayerNotesValue(bool manual)
        {
            _playerData.autoNotes = manual;
            _dataHandler.SavePlayerData(_playerData);
        }
        
        public void UpdatePlayerTapesValue(bool manual)
        {
            _playerData.autoTapes = manual;
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