/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Runtime.Managers;
using Runtime.SaveSystem.Data;
using UnityEngine.SceneManagement;

namespace Runtime.SaveSystem
{
    [DefaultExecutionOrder(-1)]
    public class SaveManager : MonoBehaviour
    {
        [Header("SAVE SYSTEM CONFIG")]
        [SerializeField] private string saveFileName;
        [SerializeField] private bool useEncryption;
        
        [SerializeField] private Vector3 playerStartPosition;
        
        [HideInInspector] public bool saveExists;

        private List<IPersistant> _persistantObjects;
        private FileHandler _dataHandler;
        private SaveData _saveData;

        // ======================================= UNITY METHODS =======================================
        
        private void Awake() 
        {
            var saveGamePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
            saveGamePath += "/My Games/Sector Zero/";
            _dataHandler = new FileHandler(saveGamePath, saveFileName, useEncryption ? EncryptionType.AES : EncryptionType.None);
            saveExists = _dataHandler.HasSaveFile();
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
            LoadData();
        }
        
        // ======================================= PUBLIC METHODS =======================================

        public void LoadGame()
        {
            _saveData = _dataHandler.Load();
            SceneManager.LoadScene(_saveData.currentScene);
        }

        public void NewGame() 
        {
            DeleteSaveData();
            saveExists = false;
            _saveData = new SaveData();

            if (GameManager.Instance.testMode)
            {
                _saveData.playerData.position = playerStartPosition;
            }
        }

        public void SaveGame()
        {
            if (_saveData == null) 
            {
                Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
                return;
            }
            
            foreach (var persistentObject in _persistantObjects)
            {
                persistentObject.SaveData(_saveData);
            }
            
            _saveData.currentScene = SceneManager.GetActiveScene().buildIndex;
            _dataHandler.Save(_saveData);
            saveExists = true;
        }
        
        // ======================================= PRIVATE METHODS =======================================
        
        private void LoadData()
        {
            foreach (var persistantObject in _persistantObjects)
            {
                persistantObject.LoadData(_saveData);
            }
        }

        private static List<IPersistant> FindAllPersistenceObjects()
        {
            var dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IPersistant>();
            return new List<IPersistant>(dataPersistenceObjects);
        }

        private void DeleteSaveData()
        {
            _dataHandler.ClearSaves();
        }
    }
}