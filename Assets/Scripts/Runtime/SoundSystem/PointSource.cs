// Copyright (c) 2023 AteBit Games
using UnityEngine;
using Runtime.SoundSystem.Interfaces;
using Runtime.SoundSystem.ScriptableObjects;

namespace Runtime.SoundSystem
{
    public class PointSource : CustomAudioSource 
    {
        protected AudioCueInstance instance;
        
        public override bool isPlaying => instance;
        
        public override void Play()
        {
            if(loop && isPlaying)
            {
                instance.Stop(false);
            }
		
            if(audioCue != null)
            {
                if (audioCue.spatialization == AudioCue.Spatializations.Simple2D)
                {
                    instance = AudioManager.Play(audioCue, transform, Vector3.zero, loop);
                }

                if(instance)
                {
                    instance.Volume = volume;
                    instance.Pitch = pitch;
                }
            }
        }
        
        public override void Stop(bool stopImmediately)
        {
            instance.Stop(stopImmediately);
        }
        
        protected override void OnVolumePitchChanged()
        {
            if(instance)
            {
                instance.Volume = volume;
                instance.Pitch = pitch;
            }
        }
    }
}