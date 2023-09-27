using System;
using Runtime.Managers;
using UnityEngine;

namespace Runtime.SaveSystem
{
    public class SaveDebugger : MonoBehaviour
    {
        private bool _isTriggered;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P ) && !_isTriggered && GameManager.Instance.TestMode)
            {
                GameManager.Instance.SaveSystem.SaveGame();
                _isTriggered = true;
            }
        }
    }
}