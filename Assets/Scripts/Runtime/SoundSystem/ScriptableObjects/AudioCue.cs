// Copyright (c) 2023 AteBit Games
using UnityEngine;

namespace Runtime.SoundSystem.ScriptableObjects
{
	[CreateAssetMenu(fileName = "NewAudioCue", menuName = "Sound System/AudioCue")]
	public class AudioCue : ScriptableObject
    {
	    public enum FalloffTypes
        {
            Linear,
            Logarithmic,     
        };
        
        public enum Spatializations
        {
            Simple2D,
            Local3D,
            Occludable3D,	
        };
        
        [Space(15)]
        [Header("Main Properties")]
        public AudioMixGroup group;
        public AudioClip clip;
		[Tooltip("The volume of this Cue.")]
		[Range(0f, 1f)] public float volume = 1f;
		[Tooltip("The pitch adjustment of this Cue.")]
		[Range(0f, 1f)] public float pitch = 1f;
		[Tooltip("Set to true to auto-loop this Cue.")]
		public bool loops;

		
		[Space(15)]
		[Header("Spatialization Properties")]
		[Tooltip("Sets rules for how to spatialize this sound.")]
		public Spatializations spatialization = Spatializations.Occludable3D;
		[Tooltip("Attenuation style of this clip.")]
		public FalloffTypes falloff = FalloffTypes.Linear;
		[Tooltip("Amount of occlusion applied to this Cue (when occluded).")]
		[Range(0f, 1f)] public float occlusionScale = 1f;
		[Tooltip("The range at which the sound is no longer audible.")]
		public float maxDistance = 100;
		[Tooltip("The range within which the sound will be at peak volume/loudness.")]
		public float minDistance = 10;


		[Space(15)]
		[Header("Extra Properties")]
		[Tooltip("Cue priority, lower is more important.")]
		[Range(0, 256)] public int priority = 128;
		[Tooltip("Prevent this Cue from receiving Audio Effects.")]
		public bool bypassEffects;
		[Tooltip("Number of seconds over which to fade in the Cue when played.")]
		public float fadeInTime;
		[Tooltip("Number of seconds over which to fade out the Cue when stopped.")]
		public float fadeOutTime;
		[Tooltip("Number of seconds to keep this Audio Cue alive after it has stopped playing. (Used to allow effects like reverb etc. to play out before destroying the Audio Source.)")]
		public int keepAliveTime;
		[Tooltip("Expands or narrows the range of speakers out of which this Cue plays.")] 
		public float spread;

		public AudioMixGroup Group
		{
			set
			{
				if (Group != value)
				{
					if (group != null)
						group.RemoveCue(this);

					group = value;

					if (group != null)
						group.AddCue(this);
				}
			}
			get => group;
		}
		
		public bool Is3D => spatialization != Spatializations.Simple2D;
		public bool IsLocal => spatialization == Spatializations.Simple2D;

		private void OnEnable()
		{
			if(Group != null)
				Group.AddCue(this);
		}

		private void OnDisable()
		{
			if(Group != null)
				Group.RemoveCue(this);
		}
    }
}

