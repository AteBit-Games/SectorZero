/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using UnityEngine;

namespace Runtime.SoundSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewSound", menuName = "Sound System/Sound")]
    public class Sound : ScriptableObject
    {
        public SoundMixGroup mixGroup;
        public AudioClip clip;

        [Range(0.0f, 1.0f)]
        public float volume = 1f;
        
        public bool loop;

        private void OnEnable()
        {
            if (mixGroup != null) mixGroup.Sounds.Add(this);
        }
        
        private void OnDisable()
        {
            if (mixGroup != null) mixGroup.Sounds.Remove(this);
        }
    }
}
