/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using UnityEngine;

namespace Runtime.SoundSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Sound", menuName = "Sound System/Sound")]
    public class Sound : ScriptableObject
    {
        public AudioClip clip;
        public bool loop;
        
        [Range(0f, 1f)]
        public float volume = 1f;
        
        [Range(0.1f, 3f)]
        public float pitch = 1f;
    }
}
