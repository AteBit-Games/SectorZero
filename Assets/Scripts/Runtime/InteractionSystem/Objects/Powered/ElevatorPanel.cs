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
    [DefaultExecutionOrder(6)]
    public class ElevatorPanel : MonoBehaviour, IInteractable, IPowered
    {
        [SerializeField] private CustomLight customLight;
        [SerializeField] private Sound elevatorSound;
        
        [SerializeField] private Sound offSound;
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        
        [SerializeField] private NormalDoor door;

        //----- Interface Properties -----//
        private bool _isPowered;

        public bool IsPowered { get; set; }

        //----- Private Variables -----//
        private AudioSource _audioSource;

        //========================= Unity events =========================//

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            GameManager.Instance.SoundSystem.SetupSound(_audioSource, interactSound);
        }

        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            GameManager.Instance.SoundSystem.PlayOneShot(interactSound, _audioSource);
            GameManager.Instance.EndGame();
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            GameManager.Instance.SoundSystem.PlayOneShot(offSound, _audioSource);
        }

        public bool CanInteract()
        {

            return _isPowered;
        }

        //========================= Public methods =========================//
        
        public void PowerOn(bool load)
        {
            _isPowered = true;
            GameManager.Instance.SoundSystem.Play(elevatorSound, _audioSource);
            door.canInteract = true;
            customLight.PowerOn(false);
        }

        public void PowerOff()
        {
            _isPowered = false;
            _audioSource.Stop();
            door.canInteract = false;
            customLight.PowerOff();
        }
    }
}
