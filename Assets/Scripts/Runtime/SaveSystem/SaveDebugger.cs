using System;
using Runtime.Managers;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.SaveSystem
{
    public class SaveDebugger : MonoBehaviour
    {
        private bool _isSaveTriggered;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P) && !_isSaveTriggered && GameManager.Instance.testMode)
            {
                GameManager.Instance.SaveSystem.SaveGame();
                _isSaveTriggered = true;
            }
            
            if (Input.GetKeyDown(KeyCode.O) && GameManager.Instance.testMode)
            {
                GameManager.Instance.EndGame();
            }
        }
    }
}