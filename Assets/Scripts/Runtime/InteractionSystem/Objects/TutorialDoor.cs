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