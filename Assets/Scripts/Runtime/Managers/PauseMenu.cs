/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Managers
{
    public class PauseMenu : MonoBehaviour
    {
        #region Header BUTTONS
        [Space(10)]
        [Header("BUTTON REFERENCES")]
        #endregion
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button quitButton;
        
        #region Header ASSET REFERENCES
        [Space(10)]
        [Header("UI REFERENCES")]
        #endregion
        [SerializeField] private GameObject optionsMenu;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private Sound clickSound;
        
        private void Start()
        {
            resumeButton.onClick.AddListener(GameManager.Instance.HandleResume);
            quitButton.onClick.AddListener(delegate { GameManager.Instance.GoToMainMenu(clickSound); });
        }
        
        public void Pause()
        {
            Time.timeScale = 0; 
            gameObject.SetActive(true);
        }
        
        public void Resume()
        {
            Time.timeScale = 1;
            GameManager.Instance.ClickSound(clickSound);
            gameObject.SetActive(false);
        }
    }
}