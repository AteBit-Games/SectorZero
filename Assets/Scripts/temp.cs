
using Runtime.DialogueSystem;
using Runtime.Managers;
using UnityEngine;

[DefaultExecutionOrder(999)]
public class temp : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;
    private bool started;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!started)
            {
                GameManager.Instance.DialogueSystem.StartDialogue(dialogue);
                started = true;
            }
        }
    }
}
