/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.DialogueSystem;
using Runtime.InteractionSystem.Interfaces;
using Runtime.InventorySystem;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.InteractionSystem.Items
{
    public class TapeRecorder : MonoBehaviour, IInteractable, IPersistant
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private string persistentID;
        [SerializeField] private Tape tape;

        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.DialogueSystem.StartDialogue(tape.dialogue);
            gameObject.SetActive(false);
            
            GameManager.Instance.SoundSystem.Play(interactSound, transform.GetComponent<AudioSource>());
            
            var inventory = player.GetComponentInParent<PlayerInventory>();
            return inventory.AddTapeToInventory(tape);
        }

        public void OnInteractFailed(GameObject player)
        {
            throw new System.NotImplementedException();
        }
        
        public bool CanInteract()
        {
            return true;
        }

        //========================= Save System =========================//
        
        public void LoadData(SaveData data)
        {
            data.worldData.tapeRecorders.Add(persistentID, gameObject.activeSelf);
        }
        
        public void SaveData(SaveData data)
        {
            data.worldData.tapeRecorders[persistentID] = gameObject.activeSelf;
        }
    }
}
