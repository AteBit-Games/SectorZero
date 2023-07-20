/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using Runtime.Managers;
using UnityEngine;

namespace Runtime.Player
{
    public class Nellient : MonoBehaviour
    {
        private static readonly int Up = Animator.StringToHash("sitUp");

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void SitUp()
        {
            animator.SetTrigger(Up);
        }
        
        public void StartGame()
        {
            TutorialManager.TriggerEvent("TutorialStage3");
            gameObject.SetActive(false);
        }
    }
}
