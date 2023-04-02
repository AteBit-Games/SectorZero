// Copyright (c) 2023 AteBit Games
using UnityEngine;

namespace Runtime.SoundSystem.Interfaces
{
    public interface IAudioInstance 
    {
        int Generation { get; }
        bool Active { get; }
        Vector3 Position { get; set; }
        Vector3 LocalPosition { get; set; }  
        float Volume { get; set; }
        float Pitch { get; set; }
        bool Mute { get; set; }
        bool Pause { get; set; }
        int TimeSamples { get; set; }
        float TimeSeconds { get; set; }
        void Stop(bool stopImmediately);
        void ForceOcclusion(bool occluded);
        AudioSource GetInternalAudioSource();
    }
    
    /*
     * A handle to instances of AudioCue.
     *
     * Basically, whenever you play an AudioCue, you get back an instance of this class.
     * This instance serves as a handle with which to manipulate the instance after initial playback if needed.
     * You can ignore this return value, when the sound is played as a once off "fire and forget".
     * IF you have a looping sound, they wont stop themselves until the game ends, so you can use this class to stop them.
     */
    public readonly struct AudioCueInstance : IAudioInstance
    {
        private readonly IAudioInstance _internalInstance;

        public AudioCueInstance(IAudioInstance internalInstance, int generation)
        {
            _internalInstance = internalInstance;
            Generation = generation;
        }
        
        public int Generation { get; }
        
        /// Does this instance refer to an active and valid sound, or is it a dead handle.
        public bool Active => _internalInstance != null && (Generation == _internalInstance.Generation) && _internalInstance.Active;
        
        // Used to access the instance's world space position.
        public Vector3 Position
        {
            get => Active ? _internalInstance.Position : Vector3.zero;
            set
            {
                if (Active)
                {
                    _internalInstance.Position = value;
                }
            }
        }
        
        // Used to access the instance's local space position.
        public Vector3 LocalPosition
        {
            get => Active ? _internalInstance.LocalPosition : Vector3.zero;
            set
            {
                if (Active)
                {
                    _internalInstance.LocalPosition = value;
                }
            }
        }
        
        // Used to access the instance's volume.
        public float Volume
        {
            get => Active ? _internalInstance.Volume : 0;
            set
            {
                if (Active)
                {
                    _internalInstance.Volume = value;
                }
            }
        }
        
        // Used to access the instance's pitch.
        public float Pitch
        {
            get => Active ? _internalInstance.Pitch : 0;
            set
            {
                if (Active)
                {
                    _internalInstance.Pitch = value;
                }
            }
        }
        
        // Used to access the instance's mute state.
        public bool Mute
        {
            get => Active && _internalInstance.Mute;
            set
            {
                if (Active)
                {
                    _internalInstance.Mute = value;
                }
            }
        }
        
        // Used to access the instance's pause state.
        public bool Pause
        {
            get => Active && _internalInstance.Pause;
            set
            {
                if (Active)
                {
                    _internalInstance.Pause = value;
                }
            }
        }
        
        // Used to access the instance's playback time in samples.
        public int TimeSamples
        {
            get => Active ? _internalInstance.TimeSamples : 0;
            set
            {
                if (Active)
                {
                    _internalInstance.TimeSamples = value;
                }
            }
        }
        
        // Used to access the instance's playback time in seconds.
        public float TimeSeconds
        {
            get => Active ? _internalInstance.TimeSeconds : 0;
            set
            {
                if (Active)
                {
                    _internalInstance.TimeSeconds = value;
                }
            }
        }
        
        // Used to stop the instance.
        // If stopImmediately is true, the sound will stop immediately, otherwise it will fade out.
        public void Stop(bool stopImmediately)
        {
            if(Active)
            {
                _internalInstance.Stop(stopImmediately);
            }
        }
        
        // Used to force the instance to be occluded.
        public void ForceOcclusion(bool occluded)
        {
            if(Active)
            {
                _internalInstance.ForceOcclusion(occluded);
            }
        }

        // Used to access the instance's internal AudioSource.
        public AudioSource GetInternalAudioSource()
        {
            return Active ? _internalInstance.GetInternalAudioSource() : null;
        }
        
        public static implicit operator bool(AudioCueInstance x) 
        {
            return x.Active;
        }
    }

}
