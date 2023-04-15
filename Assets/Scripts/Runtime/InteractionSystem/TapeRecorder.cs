/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.DialogueSystem;
using Runtime.InventorySystem;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.SoundSystem;
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
            GameManager.GameManager.Instance.dialogueSystem.StartDialogue(dialogue);
            Destroy(gameObject);
            
            GameManager.GameManager.Instance.soundManager.Play(interactSound);
            
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
