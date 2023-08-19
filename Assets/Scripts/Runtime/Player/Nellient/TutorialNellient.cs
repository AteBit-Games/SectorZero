/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using UnityEngine;

namespace Runtime.Player.Nellient
{
    public class TutorialNellient : MonoBehaviour, IPersistant
    {
        [SerializeField] public string persistentID;

        private Animator animator;
        private static readonly int Up = Animator.StringToHash("sitUp");

        //============================= Unity Events =============================//

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }
        
        //============================= Public Methods =============================//
        
        public void SitUp()
        {
            animator.SetTrigger(Up);
        }
        
        public void StartGame()
        {
            TutorialManager.TriggerEvent("TutorialStage3");
            gameObject.SetActive(false);
        }
        
        //============================= Save System =============================//
        
        public void LoadData(SaveData data)
        {
            gameObject.SetActive(data.tutorialData.nellientState);
        }

        public void SaveData(SaveData data)
        {
            data.tutorialData.nellientState = gameObject.activeSelf;
        }
    }
}