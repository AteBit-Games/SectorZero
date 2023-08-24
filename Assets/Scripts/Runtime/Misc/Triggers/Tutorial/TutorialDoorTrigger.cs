/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.Managers;
using Runtime.Managers.Tutorial;
using UnityEngine;

namespace Runtime.Misc.Triggers.Tutorial
{
    public class TutorialDoorTrigger : MonoBehaviour
    {
        [SerializeField] private string tutorialStage;
        private Collider2D triggerDoor;

        private void Awake()
        {
            triggerDoor = GetComponent<Collider2D>();
            TutorialManager.StartListening(tutorialStage, Trigger);
        }
        
        private void Trigger()
        {
            triggerDoor.enabled = true;
        }
    }
}
