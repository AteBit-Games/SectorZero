/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
using Cinemachine;
using Runtime.InteractionSystem.Objects.Doors;
using Runtime.Managers;
using Runtime.Player;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects
{
    [DefaultExecutionOrder(5)]
    public class Elevator : MonoBehaviour
    {
        [SerializeField] private Sound rumbleSound;
        [SerializeField] private ElevatorDoor door;
        [SerializeField] private PlayerController player;
        [SerializeField] private CinematicManager cinematicManager;
        [SerializeField] private Animator animator;
        
        private AudioSource _audioSource;
        private static readonly int Fade = Animator.StringToHash("fade");

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            GameManager.Instance.SoundSystem.SetupSound(_audioSource, rumbleSound);
        }

        public void Trigger(int choice)
        {
            GameManager.Instance.SoundSystem.Play(rumbleSound, _audioSource);
            GameManager.Instance.HandleEscape();
            cinematicManager.TriggerCinematic(1);
            
            player.GetComponentInChildren<PlayerInteraction>().DisableInteraction();
            player.SetCamera(true);
            player.SetFacingDirection(Vector2.up);
            player.DisableInput();
            
            door.CloseDoor();

            StartCoroutine(EndCoroutine(choice));
        }

        private IEnumerator EndCoroutine(int choice)
        {
            yield return new WaitForSecondsRealtime(rumbleSound.clip.length-3.5f);

            animator.SetTrigger(Fade);
            
            yield return new WaitForSecondsRealtime(1f);
            
            GameManager.Instance.LoadScene(choice == 0 ? "SectorThree" : "SectorZero");
        }
    }
}
