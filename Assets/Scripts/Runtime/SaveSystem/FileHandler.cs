/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using UnityEngine;
using System;
using System.IO;
using Runtime.SaveSystem.Data;
using System.Security.Cryptography;
using System.Text;

namespace Runtime.SaveSystem
{
    public enum EncryptionType
    {
        None,
        AES
    }

    public static class EncryptionUtils
    {
        private static readonly byte [] ivBytes = new byte [16];
        private static readonly byte [] keyBytes = new byte [16];

        private static void GenerateIVBytes()
        {
            var rnd = new System.Random();
            rnd.NextBytes(ivBytes);
        }

        private const string NameOfGame = "SctrZer0";

        private static void GenerateKeyBytes()
        {
            var sum = 0;
            foreach (var curChar in NameOfGame) sum += curChar;
            var rnd = new System.Random(sum);
            rnd.NextBytes( keyBytes );
        }
     
        public static string EncryptAES( string data )
        {
            GenerateIVBytes();
            GenerateKeyBytes();
     
            SymmetricAlgorithm algorithm = Aes.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor( keyBytes, ivBytes );
            byte[] inputBuffer = Encoding.Unicode.GetBytes(data);
            byte[] outputBuffer = transform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
     
            string ivString = Encoding.Unicode.GetString(ivBytes);
            string encryptedString = Convert.ToBase64String(outputBuffer);
            return ivString + encryptedString;
        }
     
        public static string DecryptAES(string text)
        {
            GenerateIVBytes();
            GenerateKeyBytes();
     
            int endOfIVBytes = ivBytes.Length / 2;
            string ivString = text.Substring(0, endOfIVBytes);
            byte[] extractedivBytes = Encoding.Unicode.GetBytes(ivString);
            string encryptedString = text.Substring(endOfIVBytes);
     
            SymmetricAlgorithm algorithm = Aes.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(keyBytes, extractedivBytes);
            byte[] inputBuffer = Convert.FromBase64String( encryptedString );
            byte[] outputBuffer = transform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
     
            string decryptedString = Encoding.Unicode.GetString(outputBuffer);
            return decryptedString;
        }
    }

    public class FileHandler
    {
        private readonly string _savePath;
        private readonly string _saveFileName;
        private readonly EncryptionType _encryptionType;
        private const string BackupExtension = ".bak";

        public FileHandler(string savePath, string saveFileName, EncryptionType encryptionType)
        {
            _savePath = savePath;
            _saveFileName = saveFileName;
            _encryptionType = encryptionType;
        }

        public SaveData Load(bool allowRestoreFromBackup = true)
        {
            var savePath = Path.Combine(_savePath, _saveFileName);
            
            SaveData saveData = null;
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

                    if (_encryptionType == EncryptionType.AES) data = DecryptData(data);

                    saveData = JsonUtility.FromJson<SaveData>(data);
                }
                catch
                {
                    if (!allowRestoreFromBackup) return saveData;
                    
                    var rollbackSuccess = AttemptRollback(savePath);
                    if (rollbackSuccess)
                    {
                        saveData = Load(false);
                    }
                    else
                    {
                        Debug.LogError("Rollback failed. Unable to load data.");
                    }
                }
            }

            return saveData;
        }

        public void Save(SaveData data)
        {
            var savePath = Path.Combine(_savePath, _saveFileName);
            var backupFilePath = savePath + BackupExtension;
            try 
            {
                // create the directory the file will be written to if it doesn't already exist
                Directory.CreateDirectory(Path.GetDirectoryName(savePath) ?? string.Empty);
                
                var dataToStore = JsonUtility.ToJson(data, true);
                if(_encryptionType == EncryptionType.AES) dataToStore = EncryptData(dataToStore);
                
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    using (var writer = new StreamWriter(stream)) 
                    {
                        writer.Write(dataToStore);
                    }
                }

                // verify the newly saved file can be loaded successfully
                var verifiedGameData = Load();
                if (verifiedGameData != null)
                {
                    File.Copy(savePath, backupFilePath, true);
                }
                else 
                {
                    throw new Exception("Save file could not be verified and backup could not be created.");
                }
            }
            catch (Exception e) 
            {
                Debug.LogError("Error occured when trying to save data to file: " + savePath + "\n" + e);
            }
        }
        
        public void ClearSaves()
        {
            var savePath = Path.Combine(_savePath, _saveFileName);
            var backupFilePath = savePath + BackupExtension;
            try
            {
                if (File.Exists(savePath))
                {
                    File.Delete(savePath);
                }
                if (File.Exists(backupFilePath))
                {
                    File.Delete(backupFilePath);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to delete save files: " + savePath + "\n" + e);
            }
        }
        
        public bool HasSaveFile()
        {
            var savePath = Path.Combine(_savePath, _saveFileName);
            return File.Exists(savePath);
        }

        private static bool AttemptRollback(string path)
        {
            var backupFilePath = path + BackupExtension;
            if (File.Exists(backupFilePath))
            {
                try
                {
                    File.Copy(backupFilePath, path, true);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to roll back to backup file: " + backupFilePath + "\n" + e);
                }
            }
            else
            {
                Debug.LogError("No backup file found at: " + backupFilePath);
            }

            return false;
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