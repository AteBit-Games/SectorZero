/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects.UI
{
    public class Safe : MonoBehaviour, IInteractable, IPersistant
    {
        [SerializeField] public string safeCode;
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        private Animator _animator;
        private bool _isOpen;
        
        //=============================== Unity Events ===============================//
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        
        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.SoundSystem.Play(interactSound);
            GameManager.Instance.HUD.OpenKeyPad(this);
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            throw new NotImplementedException();
        }
        
        public bool CanInteract()
        {
            return !_isOpen;
        }
        
        //========================= Safe =========================//

        public void OpenSafe()
        {
            Debug.Log("Open Safe");
        }
        
        //========================= Save System =========================//

        public string LoadData(SaveGame game)
        {
            if (game.worldData.safeOpen)
            {
                //TODO: Open safe
            }
            return "Safe";
        }

        public void SaveData(SaveGame game)
        {
            game.worldData.safeOpen = _isOpen;
        }
    }
}
