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
        public float volumeScale = 1f;

        [Range(0.0f, 1.0f)] 
        [HideInInspector] public float volume;

        public bool loop;

        private void OnEnable()
        {
            if (mixGroup != null) mixGroup.Sounds.Add(this);
        }
        
        private void OnDisable()
        {
            if (mixGroup != null) mixGroup.Sounds.Remove(this);
        }
        
        public Sound CreateInstance()
        {
            var instance = CreateInstance<Sound>();
            instance.clip = clip;
            instance.volumeScale = volumeScale;
            instance.loop = loop;
            return instance;
        }
    }
}
