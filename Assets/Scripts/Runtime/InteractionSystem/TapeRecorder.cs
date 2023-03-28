using Runtime.DialogueSystem;
using Runtime.InventorySystem;
using Runtime.InventorySystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.InteractionSystem
{
    public class TapeRecorder : MonoBehaviour, IInteractable
    {
        [SerializeField] private AudioClip interactSound;
        public AudioClip InteractSound => interactSound;

        [SerializeField] private Dialogue dialogue;

        public bool OnInteract(GameObject player)
        {
            GameManager.GameManager.Instance.dialogueSystem.StartDialogue(dialogue);
            Destroy(gameObject);
            var audioSource = player.GetComponent<AudioSource>();
            audioSource.PlayOneShot(interactSound);
            
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
