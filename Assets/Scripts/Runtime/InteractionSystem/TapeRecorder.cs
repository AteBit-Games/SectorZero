/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.DialogueSystem;
using Runtime.InventorySystem;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.InteractionSystem
{
    public class TapeRecorder : MonoBehaviour, IInteractable
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;

        [SerializeField] private Dialogue dialogue;

        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.DialogueSystem.StartDialogue(dialogue);
            Destroy(gameObject);
            
            GameManager.Instance.SoundSystem.Play(interactSound, transform);
            
            var inventory = player.GetComponent<PlayerInventory>();
            var tapeInstance = ScriptableObject.CreateInstance<Tape>();
            tapeInstance.itemName = dialogue.actor.Name;
            tapeInstance.itemSprite = dialogue.actor.Sprite;
            tapeInstance.itemType = ItemType.TapeRecording;
            tapeInstance.dialogue = dialogue;
            
            return inventory.AddTapeToInventory(tapeInstance);
        }
    }
}
