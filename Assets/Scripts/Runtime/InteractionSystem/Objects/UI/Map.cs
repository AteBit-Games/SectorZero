/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects
{
    public class Map : MonoBehaviour, IInteractable
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.SoundSystem.Play(interactSound);
            GameManager.Instance.HUD.OpenMap();
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            throw new System.NotImplementedException();
        }
        
        public bool CanInteract()
        {
            return true;
        }
    }
}
