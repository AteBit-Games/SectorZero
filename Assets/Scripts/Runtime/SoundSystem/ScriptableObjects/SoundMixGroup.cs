/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace Runtime.SoundSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewMixGroup", menuName = "Sound System/Sound Mix Group")]
    public class SoundMixGroup : ScriptableObject
    {
        [SerializeField] private SoundMixGroup parent;
        private readonly List<SoundMixGroup> _children = new();
        private readonly List<Sound> _sounds = new();
        
        [Range(0.0f, 1.0f)] [Tooltip("The volume of this group will be multiplied by this value")]
        public float volume = 1f;

        private void Awake()
        {
            SetVolume(volume);
        }

        public SoundMixGroup Parent
        {
            get => parent;
            set
            {
                if (parent != null)
                {
                    parent._children.Remove(this);
                }
                parent = value;
                if (parent != null)
                {
                    parent._children.Add(this);
                }
            }
        }
        
        private List<SoundMixGroup> Children => _children;
        public List<Sound> Sounds => _sounds;

        public void ResetVolume()
        {
            volume = 1f;
            foreach(var child in _children)
            {
                child.ResetVolume();
            }
        }

        public void SetVolume(float newVolume)
        {
            volume = newVolume;
            foreach(var child in _children)
            {
                child.SetVolume(newVolume);
            }
            
            foreach(var sound in _sounds)
            {
                sound.volume = newVolume;
            }
        }

        private void OnEnable()
        {
            if (parent != null) parent.Children.Add(this);
        }

        private void OnDisable()
        {
            if (parent != null) parent.Children.Remove(this);
        }
    }
}