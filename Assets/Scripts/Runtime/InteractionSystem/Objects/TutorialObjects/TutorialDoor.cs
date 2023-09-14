/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using NavMeshPlus.Components;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Runtime.InteractionSystem.Objects.TutorialObjects
{
    [DefaultExecutionOrder(6)]
    public class TutorialDoor : MonoBehaviour, IPersistant
    {
        [SerializeField] private Sound openSound;
        [SerializeField] private Sound closeSound;
        [SerializeField] private NavMeshSurface navMeshSurface;
        [SerializeField] private Collider2D navigationBlocker;
        [SerializeField] private Animator bottomAnimator;
        [SerializeField] public string persistentID;

        //----- Private Variables -----//
        private Animator _mainAnimator;
        private AudioSource _audioSource;
        private bool _open;
        
        //---- Animator Hashes -----//
        private readonly int _isOpen = Animator.StringToHash("isOpen");
        private static readonly int Hit = Animator.StringToHash("hit");
        private static readonly int Property = Animator.StringToHash("break");
        private static readonly int Open = Animator.StringToHash("open");

        //=========================== Unity Events =============================//
        
        private void Awake()
        {
            _mainAnimator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
            GameManager.Instance.SoundSystem.SetupSound(_audioSource, openSound);
        }
        
        //=========================== Public Methods =============================//
        
        public void OpenDoor()
        {
            GameManager.Instance.SoundSystem.PlayOneShot(openSound, _audioSource);
            _mainAnimator.SetBool(_isOpen, true);
            _open = true;
        }

        public void CloseDoor()
        {
            GameManager.Instance.SoundSystem.PlayOneShot(closeSound, _audioSource);
            _mainAnimator.SetBool(_isOpen, false);
            _open = false;
        }
        
        public void TriggerHit()
        {
            _mainAnimator.SetTrigger(Hit);
        }

        public void BreakDoor()
        {
            _mainAnimator.SetTrigger(Property);
        }

        public void RemoveShadowCaster()
        {
            GetComponent<ShadowCaster2D>().enabled = false;
        }

        public void TriggerBottom()
        {
            bottomAnimator.SetTrigger(Property);
        }

        public void SetBlocker(int active)
        {
            navigationBlocker.gameObject.SetActive(active > 0);
        }

        public void UpdateMesh()
        {
            navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
        }
        
        //=========================== Save System =============================//
        
        public string LoadData(SaveGame game)
        {
            if (game.worldData.doors.ContainsKey(persistentID))
            {
                if (game.worldData.doors[persistentID])
                {
                    _mainAnimator.SetTrigger(Open);
                }
            }
            
            return persistentID;
        }

        public void SaveData(SaveGame game)
        {
            if(!game.worldData.doors.ContainsKey(persistentID)) game.worldData.doors.Add(persistentID, _open);
            else game.worldData.doors[persistentID] = _open;
        }
    }
}
