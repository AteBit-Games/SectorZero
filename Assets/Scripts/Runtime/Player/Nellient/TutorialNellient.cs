/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.Managers;
using Runtime.Managers.Tutorial;
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
        
        public string LoadData(SaveGame game)
        {
            gameObject.SetActive(game.tutorialData.nellientState);
            return persistentID;
        }

        public void SaveData(SaveGame game)
        {
            game.tutorialData.nellientState = gameObject.activeSelf;
        }
    }
}
