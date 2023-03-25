using System;
using Runtime.DialogueSystem;
using Runtime.Input;
using UnityEngine;

namespace Runtime.GameManager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
        }

        [SerializeField] private InputReader inputReader;
        [SerializeField] public DialogueManager dialogueSystem;

        private bool _isPaused;

        private void Start()
        {
            inputReader.PauseEvent += HandlePause;
            inputReader.ResumeEvent += HandleResume;
        }
        
        private void HandlePause()
        {
            _isPaused = true;
            Time.timeScale = 0;
        }
        
        private void HandleResume()
        {
            _isPaused = false;
            Time.timeScale = 1;
        }
    }
}