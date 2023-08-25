/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.InteractionSystem.Objects;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using UnityEngine;

namespace Runtime.Player.Nellient
{
    public class Nellient : MonoBehaviour, IPersistant
    {
        [SerializeField] public HospitalBed bedCollider;
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
        
        public void EnableInteraction()
        {
            bedCollider.Init();
        }
        
        //============================= Save System =============================//
        
        public void LoadData(SaveGame game)
        {
            gameObject.SetActive(game.worldData.nellientState);
        }

        public void SaveData(SaveGame game)
        {
            game.worldData.nellientState = gameObject.activeSelf;
        }
    }
}
