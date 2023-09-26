/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.AI;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SoundSystem;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects
{
    /// <summary>
    /// Linked to a computer, this door will open when the computer is powered on and interacted with.
    /// </summary>
    public class LinkedDoor : MonoBehaviour
    {
        [SerializeField] private Sound openSound;

        private bool _isActivated;
        private Animator _animator;
        private NoiseEmitter _noiseEmitter;
        private static readonly int OpenDoor = Animator.StringToHash("OpenDoor");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _noiseEmitter = GetComponent<NoiseEmitter>();
        }

        public void Open()
        {
            if(_isActivated) return;
            
            GameManager.Instance.SoundSystem.Play(openSound, transform.GetComponent<AudioSource>());
            _animator.SetTrigger(OpenDoor);
            _isActivated = true;
            _noiseEmitter.EmitGlobal();
        }
    }
}
