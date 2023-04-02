// Copyright (c) 2023 AteBit Games
using UnityEngine;
using System.Collections.Generic;

namespace Runtime.SoundSystem.ScriptableObjects
{
    /* Represents the config for a single audio mix group.
     * This will be used to group audio sources together and allow you to control their properties in bulk.
     */
    [CreateAssetMenu(fileName = "NewAudioMixGroup", menuName = "Sound System/AudioMixGroup")]
    public class AudioMixGroup : ScriptableObject
    {
        [SerializeField] private AudioMixGroup parent;
        
        private readonly List<AudioMixGroup> _children = new();
        private readonly List<AudioCue> _audioCues = new();
        private float _userVolume = 1;
        private float _userPitch = 1;
        private float _effectiveVolume = 1;
        private float _effectivePitch = 1;
        private bool _muted;

        [SerializeField] 
        [Range(0.0f, 1.0f)] 
        private float volume = 1.0f;
        [SerializeField]
        [Range(0f, 2f)]
        private float pitch = 1.0f;

        public float UserVolume
        {
            set => _userVolume = value;
            get => _userVolume;
        }
        
        public float UserPitch
        {
            set => _userPitch = value;
            get => _userPitch;
        }
        
        public bool Muted
        {
            set => _muted = value;
            get => _muted;
        }
        
        public float EffectiveVolume
        {
            get => _effectiveVolume;
            set => _effectiveVolume = _muted ? 0f : Mathf.Clamp01(volume * _userVolume * value);
        }
        
        public float EffectivePitch
        {
            get => _effectivePitch;
            set => _effectivePitch = Mathf.Clamp(pitch * _userPitch * value, 0f, 2f);
        }
        
        public AudioMixGroup Parent
        {
            set
            {
                if(value != parent && value != this)
                {
                    if(parent)
                    {
                        parent._RemoveChild(this);
                    }
                    parent = value;
                    if(parent)
                    {
                        parent._AddChild(this);
                    }
                }
            }
            get { return parent; }
        }
        
        public List<AudioMixGroup> Children => _children;
        
        public bool IsAncestorOf(AudioMixGroup group)
        {
            AudioMixGroup descendant = group;
            while(descendant != null)
            {
                if(descendant == this)
                {
                    return true;
                }
                descendant = descendant.Parent;
            }
            return false;
        }
        
        public bool IsDescendantOf(AudioMixGroup group)
        {
            AudioMixGroup ancestor = Parent;
            while(ancestor != null)
            {
                if(ancestor == group)
                {
                    return true;
                }
                ancestor = ancestor.Parent;
            }
            return false;
        }
        
        public void ResetUserVolume()
        {
            _userVolume = 1f;
            int numChildren = _children.Count;
            for(int childIndex = 0; childIndex < numChildren; ++childIndex)
            {
                AudioMixGroup child = _children[childIndex];
                if(child)
                {
                    child.ResetUserVolume();
                }
            }
        }
        
        public void AddCue(AudioCue cue)
        {
            if(cue != null && !_audioCues.Contains(cue))
            {
                _audioCues.Add(cue);
                _audioCues.Sort(delegate(AudioCue x, AudioCue y) 
                {
                    if(x == null && y == null)
                    {
                        return 0;
                    }
                    if(x == null && y != null)
                    {
                        return -1;
                    }
                    if(x != null && y == null)
                    {
                        return 1;
                    }
                    else
                    {
                        return x.name.CompareTo(y.name);
                    }
                });
            }
        }

        public void RemoveCue(AudioCue cue)
        {
            _audioCues.Remove(cue);
        }

        public List<AudioCue> Cues => _audioCues;

        private void OnEnable()
        {
            if(parent)
            {
                parent._AddChild(this);
            }
        }
        
        void OnDisable()
        {
            if(parent)
            {
                parent._RemoveChild(this);
            }
        }
        
        private void _AddChild(AudioMixGroup child)
        {
            if(child != null && !_children.Contains(child))
            {
                _children.Add(child);
            }
        }

        private void _RemoveChild(AudioMixGroup child)
        {
            _children.Remove(child);
        }
    }
}

