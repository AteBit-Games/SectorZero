/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.SoundSystem.ScriptableObjects;
using NavMeshPlus.Components;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects
{
    public class TriggerDoor : MonoBehaviour, IPersistant
    {
        [SerializeField] private Sound openSound;
        [SerializeField] private Sound closeSound;
        [SerializeField] private NavMeshSurface navMeshSurface;
        [SerializeField] private Collider2D navigationBlocker;
        [SerializeField] private Animator bottomAnimator;
        [SerializeField] private bool startOpen;
        
        [SerializeField] private string persistentID;
        public string ID
        {
            get => persistentID;
            set => persistentID = value;
        }

        private Animator _mainAnimator;
        private AudioSource _audioSource;
        private bool open;
        
        // ============ Animator Hashes ============
        private readonly int _isOpen = Animator.StringToHash("isOpen");
        private static readonly int Hit = Animator.StringToHash("hit");
        private static readonly int Property = Animator.StringToHash("break");
        private static readonly int Open = Animator.StringToHash("open");

        private void Awake()
        {
            _mainAnimator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
            if (startOpen)
            {
                _mainAnimator.SetBool(_isOpen, true);
                open = true;
            }
        }
        
        public void OpenDoor()
        {
            _audioSource.volume = openSound.volumeScale;
            _audioSource.PlayOneShot(openSound.clip);
            _mainAnimator.SetBool(_isOpen, true);
            open = true;
        }

        public void CloseDoor()
        {
            _audioSource.volume = closeSound.volumeScale;
            _audioSource.PlayOneShot(closeSound.clip);
            _mainAnimator.SetBool(_isOpen, false);
            open = false;
        }
        
        public void TriggerHit()
        {
            _mainAnimator.SetTrigger(Hit);
        }

        public void BreakDoor()
        {
            _mainAnimator.SetTrigger(Property);
        }

        public void TriggerBottom()
        {
            bottomAnimator.SetTrigger(Property);
        }

        public void SetBlocker(int enabled)
        {
            navigationBlocker.gameObject.SetActive(enabled > 0);
        }

        public void UpdateMesh()
        {
            navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
        }
        
        public void LoadData(SaveData data)
        {
            if (data.worldData.doors.ContainsKey(persistentID))
            {
                if (data.worldData.doors[persistentID])
                {
                    _mainAnimator.SetTrigger(Open);
                }
            }
        }

        public void SaveData(SaveData data)
        {
            if(!data.worldData.doors.ContainsKey(persistentID)) data.worldData.doors.Add(persistentID, open);
            else data.worldData.doors[persistentID] = open;
        }
    }
}
