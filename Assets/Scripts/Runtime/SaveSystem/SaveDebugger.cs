using System;
using Runtime.Managers;
using UnityEngine;

namespace Runtime.SaveSystem
{
    public class SaveDebugger : MonoBehaviour
    {
        private bool _isSaveTriggered;
        
        private bool _isSpeedUpTimeTriggered;
        private bool _isSlowDownTimeTriggered;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P) && !_isSaveTriggered && GameManager.Instance.testMode)
            {
                GameManager.Instance.SaveSystem.SaveGame();
                _isSaveTriggered = true;
            }
            
            if (Input.GetKeyDown(KeyCode.Plus) && !_isSpeedUpTimeTriggered && GameManager.Instance.testMode)
            {
                Time.timeScale = 3;
                _isSpeedUpTimeTriggered = true;
                _isSlowDownTimeTriggered = false;
            }
            
            if (Input.GetKeyDown(KeyCode.Minus) && !_isSlowDownTimeTriggered && GameManager.Instance.testMode)
            {
                Time.timeScale = 3;
                _isSlowDownTimeTriggered = true;
                _isSpeedUpTimeTriggered = false;
            }
        }
    }
}