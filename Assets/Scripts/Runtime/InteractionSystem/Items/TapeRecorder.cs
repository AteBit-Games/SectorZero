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

        [SerializeField] private Dialogue dialogue;
        [SerializeField] private string dialogueDescription;

        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.DialogueSystem.StartDialogue(dialogue);
            gameObject.SetActive(false);
            
            GameManager.Instance.SoundSystem.Play(interactSound, transform);
            
            var inventory = player.GetComponentInParent<PlayerInventory>();
            var tapeInstance = ScriptableObject.CreateInstance<Tape>();
            // tapeInstance.itemName = dialogue.actor.Name;
            // tapeInstance.itemSprite = dialogue.actor.Sprite;
            // tapeInstance.itemType = ItemType.TapeRecording;
            // tapeInstance.itemDescription = dialogueDescription;
            // tapeInstance.dialogue = dialogue;

            return inventory.AddTapeToInventory(tapeInstance);
        }

        public void LoadData(SaveData data)
        {
            data.tapeRecorders.Add(this, gameObject.activeSelf);
        }

        public void SaveData(SaveData data)
        {
            data.tapeRecorders[this] = gameObject.activeSelf;
        }

        public bool CanInteract()
        {
            return true;
        }

        public UnityEvent OnInteractEvents { get; }
    }
}
