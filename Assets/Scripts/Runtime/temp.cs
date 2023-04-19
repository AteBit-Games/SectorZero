/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.InputSystem;
using Runtime.Managers;
using UnityEngine;

namespace Runtime.Player
{
    public class Temp : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        
        private void Start()
        {
            inputReader.SneakEvent += HandleSneak;
        }
        
        private static void HandleSneak()
        {
            GameManager.Instance.SaveSystem.SaveGame();
        }

    }
}