using System;
using Runtime.Managers;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.SaveSystem
{
    public class SaveDebugger : MonoBehaviour
    {
        [SerializeField] private HeartBeatSystem heartBeatSystem;
        private bool _isSaveTriggered;

        private int _currentRate = 60;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P) && !_isSaveTriggered && GameManager.Instance.testMode)
            {
                GameManager.Instance.SaveSystem.SaveGame();
                _isSaveTriggered = true;
            }
            
            if (Input.GetKeyDown(KeyCode.Slash) && GameManager.Instance.testMode)
            {
                heartBeatSystem.Enable();
            }
            
            if (Input.GetKeyDown(KeyCode.Equals) && GameManager.Instance.testMode)
            {
                _currentRate += 2;
                heartBeatSystem.SetHeartRateImmediately(_currentRate);
            }
            
            if (Input.GetKeyDown(KeyCode.Minus) && GameManager.Instance.testMode)
            {
                _currentRate -= 2;
                heartBeatSystem.SetHeartRateImmediately(_currentRate);
            }
            
            if (Input.GetKeyDown(KeyCode.O) && GameManager.Instance.testMode)
            {
                GameManager.Instance.EndGame();
            }
        }
    }
}