/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects.Doors
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class PuzzleDoor : Door, IPowered
    {
        public Sound warningSound;
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
            GameManager.Instance.SoundSystem.Play(warningSound, GetComponent<AudioSource>());
            yield return new WaitForSeconds(1);
            CloseDoor();
        } 
    }
}
