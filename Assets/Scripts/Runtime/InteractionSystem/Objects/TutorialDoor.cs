/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.Managers;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects
{
    public class TutorialDoor : MonoBehaviour
    {
        [SerializeField] private string tutorialStage;
        private TriggerDoor triggerDoor;

        private void Awake()
        {
            triggerDoor = GetComponent<TriggerDoor>();
            TutorialManager.StartListening(tutorialStage, Trigger);
        }
        
        private void Trigger()
        {
            triggerDoor.OpenDoor();
        }
    }
}
