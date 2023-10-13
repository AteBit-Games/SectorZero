/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.InteractionSystem.Interfaces;
using Runtime.InteractionSystem.Objects.Doors;
using Runtime.Managers;
using Runtime.Misc;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects.Powered
{
    [DefaultExecutionOrder(5)]
    public class ElevatorPanel : MonoBehaviour, IInteractable, IPowered
    {
        [SerializeField] private CustomLight customLight;
        [SerializeField] private Sound elevatorSound;
        
        [SerializeField] private Sound offSound;
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private ElevatorDoor door;
        [SerializeField] private AudioSource audioSource;

        //----- Interface Properties -----//
        private bool _isPowered;

        public bool IsPowered { get; set; }

        //========================= Unity events =========================//

        private void Awake()
        {
            GameManager.Instance.SoundSystem.SetupSound(audioSource, elevatorSound);
        }

        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.SoundSystem.PlayOneShot(interactSound, audioSource);
            GameManager.Instance.ShowChoiceUI();
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            GameManager.Instance.SoundSystem.PlayOneShot(offSound, audioSource);
        }

        public bool CanInteract()
        {

            return _isPowered;
        }

        //========================= Public methods =========================//
        
        public void PowerOn(bool load)
        {
            Debug.Log("Powering on | loaded: " + load);
            if(!load) GameManager.Instance.SoundSystem.Play(elevatorSound, audioSource);
            _isPowered = true;
            door.canInteract = true;
            customLight.PowerOn(false);
        }

        public void PowerOff()
        {
            _isPowered = false;
            audioSource.Stop();
            door.canInteract = false;
            customLight.PowerOff();
        }
    }
}
