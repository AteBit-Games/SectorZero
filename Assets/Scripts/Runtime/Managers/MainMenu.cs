/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Runtime.Managers
{
    public class MainMenu : MonoBehaviour
    {
        #region Header BUTTON
        [Space(10)]
        [Header("BUTTON REFERENCE")]
        #endregion
        [SerializeField] private Button continueButton;
        
        private void Start()
        {
            continueButton.interactable = GameManager.Instance.SaveSystem.saveExists; 
        }
        
        public void StartNewGame()
        {
            GameManager.Instance.SaveSystem.NewGame();
            GameManager.Instance.isMainMenu = false;
            SceneManager.LoadScene(1);
        }
        
        public void LoadGame()
        {
            GameManager.Instance.isMainMenu = false;
            GameManager.Instance.SaveSystem.LoadGame();
        }
        
        public void PlaySound(Sound sound)
        {
            GameManager.Instance.SoundSystem.Play(sound);
        }
        
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}