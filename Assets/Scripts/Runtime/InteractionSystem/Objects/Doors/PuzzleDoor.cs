/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
using Runtime.InteractionSystem.Interfaces;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects.Doors
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class PuzzleDoor : Door, IPowered
    {
        public bool IsPowered { get; set; }
        private Coroutine _closeDoorRoutine;
        
        //=========================== public Methods =============================//

        public void TriggerDoor(float duration)
        {
            OpenDoor();
            _closeDoorRoutine = StartCoroutine(CloseDoor(duration));
        }
        
        public void PowerOn(bool load = false)
        {
            
        }

        public void PowerOff()
        {
            if(_closeDoorRoutine != null) StopCoroutine(_closeDoorRoutine);
        }

        public void LoadDoor()
        {
            mainAnimator.SetTrigger(Open);
            SetBlocker(0);
            opened = true;
        }
        
        //=========================== Unity Events =============================//
        
        private IEnumerator CloseDoor(float delay)
        {
            yield return new WaitForSeconds(delay);
            CloseDoor();
        } 
    }
}
