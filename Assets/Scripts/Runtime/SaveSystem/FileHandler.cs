/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Runtime.Managers;
using Runtime.SaveSystem.Data;
using Runtime.Utils;

namespace Runtime.SaveSystem
{
    public enum EncryptionType
    {
        None,
        AES
    }

    public class SaveInstance
    {
        public Texture2D saveImage;
        public SaveGame saveGame;
    }

    public class FileHandler
    {
        private readonly string _savePath;
        private readonly string _saveFileExtension;
        private readonly EncryptionType _encryptionType;

        public FileHandler(string savePath, string saveFileExtension, EncryptionType encryptionType)
        {
            _savePath = savePath;
            _saveFileExtension = saveFileExtension;
            _encryptionType = encryptionType;
        }
        

        public SaveInstance Save(SaveGame data)
        {
            var saveTimeString = new DateTime(data.saveTime).ToString("yyyyMMddhhmmss");
            var saveFolder = Path.Combine(_savePath, "Save-"+saveTimeString);

            try
            {
                Directory.CreateDirectory(saveFolder);
                ScreenCapture.CaptureScreenshot(Path.Combine(saveFolder, "Save-" + saveTimeString + ".png"));
                GameManager.Instance.NotificationManager.ShowSaving();

                var saveGamePath = Path.Combine(saveFolder,"Save-" + saveTimeString + _saveFileExtension);
                var dataToStore = JsonUtility.ToJson(data, true);
                if(_encryptionType == EncryptionType.AES) dataToStore = EncryptData(dataToStore);
                
                using (var stream = new FileStream(saveGamePath, FileMode.Create))
                {
                    using (var writer = new StreamWriter(stream)) 
                    {
                        writer.Write(dataToStore);
                    }
                }
                
            }
            catch (Exception e) 
            {
                //delete the save folder if it exists
                if (Directory.Exists(saveFolder))
                {
                    Directory.Delete(saveFolder, true);
                }
                
                Debug.LogError("Error occured when trying to save data to file: " + saveFolder + "\n" + e);
            }
            
            return LoadSave(saveFolder);  
        }
        
        public void ClearSaves()
        {
            var saveFolders = Directory.GetDirectories(_savePath);
            foreach(var saveFolder in saveFolders)
            {
                try
                {
                    if (Directory.Exists(saveFolder))
                    {
                        Directory.Delete(saveFolder, true);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Error occured when trying to delete save files: " + saveFolder + "\n" + e);
                }
            }
        }

        public void SavePlayerData(PlayerData playerData)
        {
            //no encryption
            var savePath = Path.Combine(_savePath, "PlayerPrefs.txt");
            try 
            {
                // create the directory the file will be written to if it doesn't already exist
                Directory.CreateDirectory(Path.GetDirectoryName(savePath) ?? string.Empty);
                
                var dataToStore = JsonUtility.ToJson(playerData, true);
                
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    using (var writer = new StreamWriter(stream)) 
                    {
                        writer.Write(dataToStore);
                    }
                }

                // verify the newly saved file can be loaded successfully
                var verifiedPlayerData = LoadPlayerData();
                if (verifiedPlayerData == null)
                {
                    throw new Exception("Save file could not be verified and backup could not be created.");
                }
            }
            catch (Exception e) 
            {
                Debug.LogError("Error occured when trying to save data to file: " + savePath + "\n" + e);
            }
        }
        
        public PlayerData LoadPlayerData()
        {
            var savePath = Path.Combine(_savePath, "PlayerPrefs.txt");
            
            var playerData = new PlayerData();
            if (File.Exists(savePath))
            {
                try
                {
                    string data;
                    using (var stream = new FileStream(savePath, FileMode.Open))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            data = reader.ReadToEnd();
                        }
                    }

                    playerData = JsonUtility.FromJson<PlayerData>(data);
                }
                catch
                {
                    Debug.LogError("Error occured when trying to load data from file: " + savePath);
                }
            }
            else
            {
                SavePlayerData(playerData);
            }
            
            return playerData;
        }
        
        public Dictionary<Texture2D, SaveGame> LoadSaves()
        {
            var saveGames = new Dictionary<Texture2D, SaveGame>();
            var saveFolders = Directory.GetDirectories(_savePath);
            foreach(var saveFolder in saveFolders)
            {
                var saveInstance = LoadSave(saveFolder);
                saveGames.Add(saveInstance.saveImage, saveInstance.saveGame);
            }

            //sort by saveGame date newest first
            return saveGames.OrderByDescending(x => x.Value.saveTime).ToDictionary(x => x.Key, x => x.Value);
        }
        
        public void DeleteSave(long saveTime)
        {
            Debug.Log("Deleting save: " + saveTime);
            
            var saveTimeString = new DateTime(saveTime).ToString("yyyyMMddhhmmss");
            var saveFolder = Path.Combine(_savePath, "Save-"+saveTimeString);
            
            try
            {
                if (Directory.Exists(saveFolder))
                {
                    Directory.Delete(saveFolder, true);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to delete save files: " + saveFolder + "\n" + e);
            }
        }
        
        //=========================================== PRIVATE METHODS ===========================================

        private SaveInstance LoadSave(string saveFolder)
        {
            var saveGame = new SaveGame();
            var saveFile = Directory.GetFiles(saveFolder, "*" + _saveFileExtension);
            var saveImage = Directory.GetFiles(saveFolder, "*.png");
            
            if(saveFile.Length > 0)
            {
                try
                {
                    string data;
                    using (var stream = new FileStream(saveFile[0], FileMode.Open))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            data = reader.ReadToEnd();
                        }
                    }

                    if (_encryptionType == EncryptionType.AES) data = DecryptData(data);

                    saveGame = JsonUtility.FromJson<SaveGame>(data);
                }
                catch
                {
                    Debug.LogError("Error occured when trying to load data from file: " + saveFile[0]);
                }
            }
            
            if(saveImage.Length > 0)
            {
                var bytes = File.ReadAllBytes(saveImage[0]);
                var texture = new Texture2D(2, 2);
                texture.LoadImage(bytes);
                texture.Apply();
                
                return new SaveInstance
                {
                    saveGame = saveGame,
                    saveImage = texture
                };
            }
            
            return new SaveInstance
            {
                saveGame = saveGame,
                saveImage = null
            };
        }

        private string EncryptData(string data)
        {
            return EncryptionUtils.EncryptAES(data);
        }
        
        private static string DecryptData(string data)
        {
            return EncryptionUtils.DecryptAES(data);
        }
    }
}