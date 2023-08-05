/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections.Generic;
using Runtime.Managers;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.Misc.Triggers
{
    public class Trigger : MonoBehaviour
    {
        [SerializeField] private List<Sound> sounds;

        private void TriggerSound(int id)
        {
            if (id < 0 || id >= sounds.Count) Debug.LogError("Sound ID does not exist!");
            GameManager.Instance.SoundSystem.Play(sounds[id]);
        }
        
        private void TriggerRandomSound()
        {
            GameManager.Instance.SoundSystem.Play(sounds[Random.Range(0, sounds.Count)]);
        }
    }
}
