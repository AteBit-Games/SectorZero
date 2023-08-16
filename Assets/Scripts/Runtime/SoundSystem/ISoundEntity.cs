using System.Collections.Generic;
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