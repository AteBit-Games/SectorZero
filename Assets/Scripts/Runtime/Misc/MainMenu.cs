/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections.Generic;
using Runtime.Managers;
using UnityEngine;

namespace Runtime.Misc
{
    [DefaultExecutionOrder(6)]
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private List<GameObject> mainMenuList;

        private void Start()
        {
            foreach (var item in mainMenuList)
            {
                item.SetActive(false);
            }
            
            mainMenuList[GameManager.Instance.SaveSystem.GetPlayerData().ending].SetActive(true);
        }
    }
}
