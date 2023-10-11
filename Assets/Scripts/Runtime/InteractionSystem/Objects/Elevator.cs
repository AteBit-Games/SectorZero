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
        [SerializeField] private CinemachineTargetGroup targetGroup;
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

            targetGroup.m_Targets = new CinemachineTargetGroup.Target[1];
            targetGroup.m_Targets[0].target = transform;
            
            player.GetComponentInChildren<PlayerInteraction>().DisableInteraction();
            player.DisableInput();
            
            door.CloseDoor();

            StartCoroutine(EndCoroutine(choice));
        }

        private IEnumerator EndCoroutine(int choice)
        {
            yield return new WaitForSecondsRealtime(rumbleSound.clip.length-1);

            animator.SetTrigger(Fade);
            
            yield return new WaitForSecondsRealtime(1f);
            
            switch (choice)
            {
                case 0:
                    GameManager.Instance.EndGame();
                    break;
                case 1:
                    GameManager.Instance.LoadScene("SectorZero");
                    break;
            }
        }
    }
}
