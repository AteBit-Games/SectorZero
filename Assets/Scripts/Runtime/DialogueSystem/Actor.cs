/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace Runtime.DialogueSystem
{
    [CreateAssetMenu(fileName = "New Actor", menuName = "Dialogue System/Actor")]
    public class Actor : ScriptableObject
    {
        [SerializeField, Tooltip("Name of the actor")] private new string name;
        [SerializeField, Tooltip("Sprite to display in the UI")] private Sprite sprite;
        [SerializeField] private List<AudioClip> actorSounds;
        
        [Range(1, 5), SerializeField, Tooltip("Frequency of sounds being played")] private int frequency = 2;
        [Range(-3, 3), SerializeField] private float minPitch = 0.5f;
        [Range(-3, 3), SerializeField] private float maxPitch = 3.0f;
        
        [SerializeField, Tooltip("Stops previous sound before playing the next")] private bool stopAudioSource;
        [SerializeField, Tooltip("Plays sounds in a predictable order when true")] private bool useHash = true;
    
        public string Name => name;
        public Sprite Sprite => sprite;
        public List<AudioClip> ActorSounds => actorSounds;
        public int Frequency => frequency;
        public float MinPitch => minPitch;
        public float MaxPitch => maxPitch;
        public bool StopAudioSource => stopAudioSource;
        public bool UseHash => useHash;
    }
}
