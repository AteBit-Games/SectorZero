/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.SoundSystem.ScriptableObjects
{
    public enum SoundMixGroupType
    {
        Master,
        Music,
        SFX
    }
    
    [CreateAssetMenu(fileName = "NewMixGroup", menuName = "Sound System/Sound Mix Group")]
    public class SoundMixGroup : ScriptableObject
    {
        [SerializeField] private SoundMixGroupType type;
        
        private readonly List<Sound> _sounds = new();
        
        [Range(0.0f, 1.0f)] [Tooltip("The volume of this group will be multiplied by this value")]
        public float volume = 1f;
        
        public SoundMixGroupType Type => type;
        public List<Sound> Sounds => _sounds;
        
        public void SetVolume(float newVolume)
        {
            switch (type)
            {
                case SoundMixGroupType.Master:
                    volume = newVolume;
                    AudioListener.volume = newVolume;
                    break;
                
                case SoundMixGroupType.Music: case SoundMixGroupType.SFX:
                    Debug.Log($"Setting volume of {type} to {newVolume}");
                    volume = newVolume;
                    foreach (var sound in _sounds)
                    {
                        sound.volume = sound.volumeScale * newVolume;
                    }
                    break;
                
                default:
                    Debug.LogError($"Unknown SoundMixGroupType: {type}");
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}