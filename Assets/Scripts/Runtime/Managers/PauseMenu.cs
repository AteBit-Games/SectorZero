/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.Managers
{
    public class PauseMenu : MonoBehaviour
    {
        public void Pause()
        {
            Time.timeScale = 0; 
            gameObject.SetActive(true);
        }
        
        public void Resume()
        {
            Time.timeScale = 1;
            GameManager.Instance.ResetInput();
            gameObject.SetActive(false);
        }
        
        public void PlaySound(Sound sound)
        {
            GameManager.Instance.SoundSystem.Play(sound);
        }
        
        public void GoToMainMenu()
        {
            Time.timeScale = 1;
            GameManager.Instance.SoundSystem.StopAll();
            GameManager.Instance.isMainMenu = true;
            GameManager.Instance.ResetInput();
            SceneManager.LoadScene(0);
        }
        
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}