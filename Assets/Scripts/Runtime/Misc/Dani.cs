/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using Runtime.DialogueSystem;
using Runtime.Managers;
using UnityEngine;

namespace Runtime.Misc
{
    [DefaultExecutionOrder(6)]
    public class Dani : MonoBehaviour
    {
        [SerializeField] public Dialogue dialogue;

        //============================== Save System ==============================

        private void Start()
        {
            GameManager.Instance.DialogueSystem.OnDialogueFinish += TriggerEnd;
        }

        private void OnDisable()
        {
            GameManager.Instance.DialogueSystem.OnDialogueFinish -= TriggerEnd;
        }
        
        public void TriggerDialogue()
        {
            GameManager.Instance.DialogueSystem.StartDialogue(dialogue);
        }

        private void TriggerEnd()
        {
            Debug.Log("End");
        }
    }
}
