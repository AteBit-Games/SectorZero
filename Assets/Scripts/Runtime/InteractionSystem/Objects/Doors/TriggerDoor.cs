/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using NavMeshPlus.Components;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Runtime.InteractionSystem.Objects.Doors
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class TriggerDoor : MonoBehaviour, IPersistant
    {
        [SerializeField] private Sound openSound;
        [SerializeField] private Sound closeSound;
        [SerializeField] private NavMeshSurface navMeshSurface;
        [SerializeField] private Collider2D navigationBlocker;
        [SerializeField] private bool startOpen;
        [SerializeField] public string persistentID;

        //----- Private Variables -----//
        private Animator _mainAnimator;
        private AudioSource _audioSource;
        private bool open;
        
        //---- Animator Hashes -----//
        private readonly int _isOpen = Animator.StringToHash("isOpen");
        private static readonly int Open = Animator.StringToHash("open");

        //=========================== Unity Events =============================//
        
        private void Awake()
        {
            _mainAnimator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
        }
        
        
        //=========================== Public Methods =============================//
        
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
        
        public void RemoveShadowCaster()
        {
            GetComponent<ShadowCaster2D>().enabled = false;
        }
        
        public void SetBlocker(int enabled)
        {
            navigationBlocker.gameObject.SetActive(enabled > 0);
        }

        public void UpdateMesh()
        {
            navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
        }
        
        
        //=========================== Save System =============================//
        
        public void LoadData(SaveGame game)
        {
            if (game.worldData.doors.ContainsKey(persistentID))
            {
                if(game.worldData.doors[persistentID])
                {
                    _mainAnimator.SetTrigger(Open);
                    _mainAnimator.SetBool(_isOpen, true);
                    open = true;
                }
            }
            else
            {
                if(startOpen)
                {
                    _mainAnimator.SetTrigger(Open);
                    _mainAnimator.SetBool(_isOpen, true);
                    open = true;
                }
            }
        }

        public void SaveData(SaveGame game)
        {
            if(!game.worldData.doors.ContainsKey(persistentID)) game.worldData.doors.Add(persistentID, open);
            else game.worldData.doors[persistentID] = open;
        }
    }
}
