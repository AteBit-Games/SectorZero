// Copyright (c) 2023 AteBit Games
using System.Collections.Generic;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.SoundSystem
{
    /* Defines the manager for ambient audio.
     * The ambient audio manager is responsible for playing ambient audio while not being attached to any specific object.
     */
    [System.Serializable]
    public class AmbientAudio
    {
        [Tooltip("The looping 2D cue to play as long as this ambience is active.")]
        public AudioCue backgroundLoop;
        [Tooltip("A list of one-shots that will play randomly around the listener.")]
        public List<AudioCue> oneShots = new();
        [Tooltip("The min and max time between one-shot playback.")]
        public Vector2 oneShotInterval = new(30f, 60f);
        [Tooltip("The a volume scalar for the Cues in this Ambience. Combines with the base Cue volume.")]
        public float volume = 1f;
        [Tooltip("When enabled the higher probability one shots will be more likely to play, gets the probability from the cue.")]
        public bool useOneShotCuesProbability = true;
        [HideInInspector]
        public float totalProbability;
    }
}

