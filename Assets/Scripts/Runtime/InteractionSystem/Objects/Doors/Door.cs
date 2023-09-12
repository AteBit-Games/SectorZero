/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using NavMeshPlus.Components;
using Runtime.Managers;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects.Doors
{
    [DefaultExecutionOrder(6)]
    public class Door : MonoBehaviour
    {
        [SerializeField] private Sound openSound;
        [SerializeField] private Sound closeSound;
        [SerializeField] protected NavMeshSurface navMeshSurface;
        [SerializeField] protected Collider2D navigationBlocker;
        [SerializeField] protected bool startOpen;

        //----- Private Variables -----//
        protected Animator mainAnimator;
        protected AudioSource audioSource;
        protected bool opened;
        
        //---- Hashes -----//
        protected readonly int isOpen = Animator.StringToHash("isOpen");
        protected static readonly int Open = Animator.StringToHash("open");

        //=========================== Unity Events =============================//
        
        private void Awake()
        {
            mainAnimator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            GameManager.Instance.SoundSystem.SetupSound(audioSource, openSound);
        }
        
        //=========================== Public Methods =============================//
        
        public void OpenDoor()
        {
            audioSource.volume = openSound.volumeScale;
            GameManager.Instance.SoundSystem.PlayOneShot(openSound, audioSource);
            
            mainAnimator.SetBool(isOpen, true);
            opened = true;
        }

        public void CloseDoor()
        {
            audioSource.volume = closeSound.volumeScale;
            GameManager.Instance.SoundSystem.PlayOneShot(closeSound, audioSource);
            
            mainAnimator.SetBool(isOpen, false);
            opened = false;
        }

        protected void SetBlocker(int enabled)
        {
            navigationBlocker.gameObject.SetActive(enabled > 0);
            navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
        }

        //=========================== Private Events =============================//

        protected void DisableInteraction()
        {
            gameObject.layer = 0;
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
