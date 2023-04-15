/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.DialogueSystem;
using Runtime.InventorySystem;
using Runtime.InputSystem;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.GameManager
{
    [ExecuteInEditMode]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
        }

        [SerializeField] private InputReader inputReader;
        [SerializeField] public DialogueManager dialogueSystem;
        [SerializeField] public InventoryManager inventorySystem;
        [SerializeField] public SoundManager soundManager;

        private void Start()
        {
            inputReader.PauseEvent += HandlePause;
            inputReader.ResumeEvent += HandleResume;
            inputReader.OpenInventoryEvent += OpenInventoryWindow;
            inputReader.CloseInventoryEvent += CloseInventoryWindow;
        }

        private void CloseInventoryWindow()
        {
            Time.timeScale = 1;
            inventorySystem.CloseInventory();
        }

        private void OpenInventoryWindow()
        {
            Time.timeScale = 0;
            inventorySystem.OpenInventory();
        }

        private void HandlePause()
        {
            Time.timeScale = 0;
        }
        
        private void HandleResume()
        {
            Time.timeScale = 1;
        }
    }
}