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
        [Header("BUTTON REFERENCES")]
        #endregion
        [SerializeField] private Button continueButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button closeOptionsButton;
        
        #region Header ASSET REFERENCES
        [Space(10)]
        [Header("UI REFERENCES")]
        #endregion
        [SerializeField] private GameObject optionsMenu;
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private Sound clickSound;
        
        private void Start()
        {
            continueButton.interactable = GameManager.Instance.SaveSystem.saveExists;
            continueButton.onClick.AddListener(LoadGame);
            newGameButton.onClick.AddListener(StartNewGame);
            optionsButton.onClick.AddListener(OpenOptions);
            closeOptionsButton.onClick.AddListener(CloseOptions);
            quitButton.onClick.AddListener(delegate { GameManager.Instance.QuitGame(clickSound); });
        }
        
        private void LoadGame()
        {
            GameManager.Instance.PlayGame(clickSound);
            GameManager.Instance.SaveSystem.LoadGame();
        }

        private void OpenOptions()
        {
            optionsMenu.SetActive(true);
            mainMenu.SetActive(false);
            GameManager.Instance.ClickSound(clickSound);
        }

        private void CloseOptions()
        {
            optionsMenu.SetActive(false);
            mainMenu.SetActive(true);
            GameManager.Instance.ClickSound(clickSound);
        }

        private void StartNewGame()
        {
            GameManager.Instance.PlayGame(clickSound);
            GameManager.Instance.SaveSystem.NewGame();
        }
    }
}