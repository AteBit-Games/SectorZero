/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using NavMeshPlus.Components;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects.Doors
{
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
        }
        
        //=========================== Public Methods =============================//
        
        public void OpenDoor()
        {
            audioSource.volume = openSound.volumeScale;
            audioSource.PlayOneShot(openSound.clip);
            
            mainAnimator.SetBool(isOpen, true);
            opened = true;
        }

        public void CloseDoor()
        {
            audioSource.volume = closeSound.volumeScale;
            audioSource.PlayOneShot(closeSound.clip);
            
            mainAnimator.SetBool(isOpen, false);
            opened = false;
        }

        protected void SetBlocker(int enabled)
        {
            navigationBlocker.gameObject.SetActive(enabled > 0);
        }

        //=========================== Private Events =============================//

        protected void DisableInteraction()
        {
            gameObject.layer = 0;
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
