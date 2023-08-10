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
        [SerializeField] private string persistentID;
        public string ID
        {
            get => persistentID;
            set => persistentID = value;
        }
        
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
