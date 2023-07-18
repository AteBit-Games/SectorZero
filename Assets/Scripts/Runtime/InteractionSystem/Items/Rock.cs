/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.AI;
using Runtime.DialogueSystem;
using Runtime.InteractionSystem.Interfaces;
using Runtime.InventorySystem;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.InteractionSystem.Items
{
    public class Rock : MonoBehaviour, IInteractable, IPersistant, IThrowable
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private Sound dropSound;
        public Sound DropSound => dropSound;
        
        
        public bool OnInteract(GameObject player)
        {
            gameObject.SetActive(false);
            GameManager.Instance.SoundSystem.Play(interactSound, transform);
            //GameManager.Instance.HUD.SetThrowableIcon();
            
            GameManager.Instance.HUD.ShowThrowableIcon(true);

            var inventory = player.GetComponentInParent<PlayerInventory>();
            inventory.PickUpThrowable(gameObject);

            return true;
        }

        public void LoadData(SaveData data)
        {
            //data.tapeRecorders.Add(this, gameObject.activeSelf);
        }

        public void SaveData(SaveData data)
        {
            //data.tapeRecorders[this] = gameObject.activeSelf;
        }

        public bool CanInteract()
        {
            return true;
        }
        
        public void OnDrop(Transform dropPosition)
        {
            GameManager.Instance.SoundSystem.Play(DropSound, transform);
            GetComponent<NoiseEmitter>().EmitLocal();
        }
    }
}
