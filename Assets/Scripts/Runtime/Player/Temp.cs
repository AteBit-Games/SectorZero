using Runtime.DialogueSystem;
using Runtime.GameManager;
using Runtime.Input;
using UnityEngine;

public class Temp : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private InputReader inputReader;

    private void Awake()
    {
        inputReader.InteractEvent += StartDialogue;
    }
    
    private void StartDialogue()
    {
        GameManager.Instance.dialogueSystem.StartDialogue(dialogue);
    }
}
