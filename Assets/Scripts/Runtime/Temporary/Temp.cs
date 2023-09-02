/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.Managers;
using UnityEngine;

namespace Runtime.Temporary
{
    public class Temp : MonoBehaviour
    {
        private bool isTriggered;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P) && !isTriggered && GameManager.Instance.TestMode)
            {
                GameManager.Instance.SaveSystem.SaveGame();
                isTriggered = true;
            }
        }

        public void EndGame()
        {
            GameManager.Instance.EndGame();
        }
    }
}