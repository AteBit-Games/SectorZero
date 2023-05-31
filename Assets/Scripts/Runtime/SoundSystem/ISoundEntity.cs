using System.Collections.Generic;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.SoundSystem
{
    public interface ISoundEntity
    {
        public AudioSource AudioSource { get;}
        public float VolumeScale { get; }
        public float Volume { get; set; }
    }
}