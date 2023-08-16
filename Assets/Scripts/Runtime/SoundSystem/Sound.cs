/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using UnityEngine;
using UnityEngine.Audio;

namespace Runtime.SoundSystem
{
    [CreateAssetMenu(fileName = "NewSound", menuName = "Sound System/Sound")]
    public class Sound : ScriptableObject
    {
        public AudioMixerGroup mixerGroup;
        public AudioClip clip;

        [Range(0.0f, 1.0f)]
        public float volumeScale = 1f;
        

        public bool loop;
    }
}
