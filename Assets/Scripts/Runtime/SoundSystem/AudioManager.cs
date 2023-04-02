// Copyright (c) 2023 AteBit Games
#region Imports

using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using Runtime.SoundSystem.Interfaces;
using Runtime.SoundSystem.ScriptableObjects;
using Random = UnityEngine.Random;

#endregion

namespace Runtime.SoundSystem
{
    [RequireComponent(typeof(AudioListener))]
    [ExecuteInEditMode]
    public class AudioManager : MonoBehaviour
    {
        #region Private Details

        private class Instance : IAudioInstance
        {
	        #region Interface Implementation
	        public int Generation => _generation;

	        public bool Active
	        {
		        get
		        {
#if UNITY_EDITOR
			        if (!EditorApplication.isPaused)
			        {
#endif

				        return (Loops || Delayed || (_source && (_source.isPlaying || Paused))) && !FadingOut;

#if UNITY_EDITOR
			        }
			        else
			        {
				        return true;
			        }
#endif
		        }
	        }

	        public Vector3 Position
	        {
		        get
		        {
			        Vector3 worldSpacePosition = _localPosition;
			        if(_parent)
			        {
				        if(_3D && Local)
				        {
					        worldSpacePosition += _parent.transform.position;
				        }
				        else
				        {
					        worldSpacePosition = _parent.localToWorldMatrix.MultiplyPoint3x4(worldSpacePosition);
				        }
			        }
			        return worldSpacePosition;
		        }
		        set
		        {
			        if (_parent)
			        {
				        if (_3D && Local)
				        {
					        _localPosition = value - _parent.transform.position;
				        }
				        else
				        {
					        _localPosition = _parent.worldToLocalMatrix.MultiplyPoint3x4(value);
				        }
			        }
			        else
			        {
				        _localPosition = value;
			        }

			        if(_source)
			        {
				        _source.transform.position = value;
			        }
		        }
	        }
	        
	        public Vector3 LocalPosition
	        {
		        get => _localPosition;
		        set
		        {
			        this._localPosition = value;
			        if(_source)
			        {
				        _source.transform.position = Position;
			        }
		        }
	        }
	        
	        public float Volume
	        {
		        get => _userVolume;
		        set
		        {
			        if(Math.Abs(_userVolume - value) > 0.001f)
			        {
				        _userVolume = Mathf.Clamp01(value);
				        Update(0f, true);
			        }
		        }
	        }
	        
	        public float Pitch
	        {
		        get => _userPitch;
		        set
		        {
			        if(Math.Abs(_userPitch - value) > 0.001f)
			        {
						_userPitch = Mathf.Clamp(value, -3f, 3f);
				        Update(0f, true);
			        }
		        }
	        }
	        
	        public bool Mute
	        {
		        get => Muted;
		        set
		        {
			        if(Muted != value)
			        {
				        _SetFlag(Flags.Muted, value);
				        if(_source)
				        {
					        _source.mute = value;
				        }
			        }
		        }
	        }
	        
	        public bool Pause
	        {
		        get => Paused;
		        set
		        {
			        if(_GetFlag(Flags.Paused) != value)
			        {
				        _SetFlag(Flags.Paused, value);
				        if(_source)
				        {
					        if(value)
					        {
						        _source.Pause();
					        }
					        else
					        {
						        _source.Play();
					        }
				        }
			        }
		        }
	        }
	        
	        public float TimeSeconds		
	        {
		        get => _source != null ? _source.time : 0f;
		        set
		        {
			        if(_source)
			        {
				        _source.time = value;
			        }
		        }
	        }

	        public int TimeSamples
	        {
		        get => _source != null ? _source.timeSamples : 0;
		        set
		        {
			        if(_source)
			        {
				        _source.timeSamples = value;
			        }
		        }
	        }
	        
	        public void ForceOcclusion(bool occluded)
	        {
		        if(_audioCue && _audioCue.spatialization == AudioCue.Spatializations.Occludable3D)
		        {
			        _SetFlag(Flags.Occluded, occluded);
		        }
	        }
	        
	        public AudioSource GetInternalAudioSource()
	        {
		        return _source;
	        }
	        
	        #endregion

	        #region Internal Interface

	        public bool Loops => (_flags & Flags.Loops) != 0;
	        public bool Local => (_flags & Flags.Local) != 0;
	        public bool _3D => (_flags & Flags.ThreeD) != 0;
	        public bool FadingIn => (_flags & Flags.FadingIn) != 0;
	        public bool FadingOut => (_flags & Flags.FadingOut) != 0;
	        public bool Muted => (_flags & Flags.Muted) != 0;
	        public bool Paused => (_flags & Flags.Paused) != 0 || AudioListener.pause;
	        public bool Occludable => (_flags & Flags.Occludable) != 0;
	        public bool Occluded => (_flags & Flags.Occluded) != 0;
	        public bool Delayed => (_flags & Flags.Delayed) != 0;

	        public AudioMixGroup Group => _audioCue != null ? _audioCue.group : null;
	        public AudioCue Cue => _audioCue;
	        public double keepAliveTimeStamp;

	        public void Init(AudioCue audioCue, Transform parent, Vector3 localPosition, bool loops)
	        {
		        if(this._audioCue == null)
		        {
			        ++_generation;
			        this._audioCue = audioCue;

			        _flags = 0;
			        _SetFlag(Flags.Loops, loops);
			        _SetFlag(Flags.Local, audioCue.IsLocal);
			        _SetFlag(Flags.ThreeD, audioCue.Is3D);
					_SetFlag(Flags.Occludable, _audioManager.occlusionMethod != 0 && audioCue.spatialization == AudioCue.Spatializations.Occludable3D);
			        
			        _userVolume = 1f;
			        _userPitch = 1f;

			        this._parent = Local ? Listener : parent;
			        this._localPosition = localPosition;
			        _ScheduleNextTest();
		        }
	        }
	        
	        public void Clone(Instance instance, Vector3 newPosition)
	        {
		        if(instance != null && instance.Active)
		        {
			        ++_generation;
			        _audioCue = instance._audioCue;
			        _flags = instance._flags;
			        _fadeStartTime = instance._fadeStartTime;
			        _basePitch = instance._basePitch;
			        _baseVolumeLoudness = instance._baseVolumeLoudness;
			        _userVolume = instance._userVolume;
			        _userPitch = instance._userPitch;
			        _occlusionAlpha = instance._occlusionAlpha;
			        _parent = instance._parent;
			        Position = newPosition;
			        
			        _ScheduleNextTest();

			        if(_AcquireSource())
			        {
				        Update(0f, true);
				        if(_source)
				        {
					        _SetFlag(Flags.Paused, false);
					        _source.clip = instance._source.clip;
					        _source.timeSamples = instance._source.timeSamples;
					        _source.Play();
				        }
			        }
		        }
	        }
	        
	        public void UnInit()
	        {
		        keepAliveTimeStamp = 0;
		        if(_audioCue != null)
		        {
			        _ReleaseSource();
			        _audioCue = null;
			        _parent = null;
			        _flags = 0;
		        }
	        }
	        
	        public void Play()
	        {
		        if(_AcquireSource())
		        {
			        if(_audioCue.clip.loadState == AudioDataLoadState.Unloaded)
			        {
				        _audioCue.clip.LoadAudioData();
			        }
			        
			        if(_audioCue.fadeInTime > 0)
			        {
				        _fadeStartTime = _currentTime;
				        _SetFlag(Flags.FadingIn, true);
				        _SetFlag(Flags.FadingOut, false);
			        }
			        if(Occludable)
			        {
				        _SetFlag(Flags.Occluded, IsOccluded(Position, _audioManager.occlusionMethod));
				        _occlusionAlpha = Occluded ? 1f : 0f;
			        }

			        _baseVolumeLoudness = _audioCue.volume;
			        _baseVolumeLoudness *= _audioCue.volume;
			        
				
			        Update(0f, true);
			        if(_source)
			        {
				        _SetFlag(Flags.Paused, false);
				        _source.clip = _audioCue.clip;
				        _source.Play();
			        }
		        }
	        }
	        
	        public void Stop(bool stopImmediately)
	        {
		        _SetFlag(Flags.Loops, false);
		        _SetFlag(Flags.Delayed, false);
		        _Stop(stopImmediately);
	        }

	        public void Update(float deltaTime, bool volumeOnly)
			{
				if(Delayed)
				{
					if(_currentTime >= _nextTestTime)
					{
						_source.Play();
						_SetFlag(Flags.Delayed, false);
						_ScheduleNextTest();
					}
					else
					{
						return;
					}
				}

				AudioCue srcCue = _audioCue;
				Vector3 worldSpacePosition;
				if(_3D)
				{
					worldSpacePosition = Position;
					if(_source)
					{
						_source.transform.position = worldSpacePosition;
					}
				}
				else
				{
					worldSpacePosition = Listener.position;
				}

				float fadeVolume = 1;
				if(FadingIn)
				{
					float fadeDelta = _currentTime - _fadeStartTime;
					fadeVolume = Mathf.Clamp01(fadeDelta / srcCue.fadeInTime);
					if(fadeVolume >= 1f)
					{
						_SetFlag(Flags.FadingIn, false);
					}
				}
				else if(FadingOut)
				{
					float fadeDelta = _currentTime - _fadeStartTime;
					fadeVolume = Mathf.Clamp01(1 - (fadeDelta / srcCue.fadeOutTime));
					if(fadeVolume <= 0f)
					{
						_SetFlag(Flags.FadingOut, false);
						_Stop(true);
					}
				}

				Vector3 listenerPos = Listener.transform.position;
				float listenerDistance = Vector3.Magnitude(worldSpacePosition - listenerPos);


				if(_source && (_source.isPlaying || Paused || volumeOnly) && !Muted)
				{
					float busVolume = _audioCue.group ? _audioCue.group.EffectiveVolume : _audioManager.masterGroup.EffectiveVolume;
					float busPitch = _audioCue.group ? _audioCue.group.EffectivePitch : _audioManager.masterGroup.EffectivePitch;
					
					const float paramVolume = 1f;
					const float paramPitch = 1f;
					float currentVolume = _baseVolumeLoudness * fadeVolume * _userVolume * paramVolume;
					
					if(Occludable)
					{
						const float occludeRate = 1f;
						_occlusionAlpha += deltaTime * (Occluded ? occludeRate : -occludeRate);
						_occlusionAlpha = Mathf.Clamp01(_occlusionAlpha);
						float effectiveOcclusion = _occlusionAlpha * srcCue.occlusionScale;
						currentVolume *= Mathf.Lerp(1f, _audioManager.occlusionVolume, effectiveOcclusion);
						if(_lowpass)
						{
							_lowpass.enabled = _occlusionAlpha > 0f;
							if(_lowpass.enabled)
							{
								_lowpass.cutoffFrequency = Mathf.Lerp(22000, _audioManager.occlusionCutoff, effectiveOcclusion);
								_lowpass.lowpassResonanceQ = Mathf.Lerp(1, _audioManager.occlusionResonanceQ, effectiveOcclusion);
							}
						}
					}

					_source.volume = Mathf.Clamp01(currentVolume * busVolume);
					_source.pitch = Mathf.Clamp(_userPitch * _basePitch * paramPitch * busPitch, 0f, 2f);
				}
				
				if(volumeOnly)
				{
					return;
				}

				// Compute the near-2D blend for this Instance.
				if(_source && (_source.isPlaying || Paused) && !Local && _audioManager.blendNearbySounds)
				{
					float spatialBlend;
					if(listenerDistance <= _audioManager.nearBlendRange.x)
					{
						spatialBlend = 0f;
					}
					else if(listenerDistance <= _audioManager.nearBlendRange.y)
			   		{
						spatialBlend = Mathf.Clamp01(listenerDistance - _audioManager.nearBlendRange.x) / (_audioManager.nearBlendRange.y - _audioManager.nearBlendRange.x);
					}
					else
					{
						spatialBlend = 1f;
					}

					_source.spatialBlend = spatialBlend;
				}
				
				if(Loops && !Paused)
				{
					bool actuallyPlaying = (_source != null) && (_source.isPlaying);
					if(Local)
					{
						if(!actuallyPlaying)
						{
							Play();
						}
					}
					else
					{
						if(_currentTime >= _nextTestTime)
						{
							bool inRange = CheckProximity(_audioCue, _parent, _localPosition);
							if(inRange && !actuallyPlaying)
							{
								Play();
							}
							else if(!inRange && actuallyPlaying)
							{
								_Stop(true);
							}
							else if(Occludable )
							{
								_SetFlag(Flags.Occluded, IsOccluded(worldSpacePosition, _audioManager.occlusionMethod));
							}
							_ScheduleNextTest();
						}
					}
				}
			}

	        #endregion
	        
            #region Private Details
            
            private int _generation;
            private AudioSource _source;
            private AudioLowPassFilter _lowpass;
            private AudioCue _audioCue;
            private Transform _parent;
            private Vector3 _localPosition = Vector3.zero;
            private Flags _flags = 0;
            private float _nextTestTime;
            private float _fadeStartTime;
            private float _basePitch = 1;
            private float _baseVolumeLoudness = 1;
            private float _userVolume = 1;
            private float _userPitch = 1;
            private float _occlusionAlpha = 1;

            [Flags]
            private enum Flags
            {
	            Loops			= 1 << 0,
	            FadingIn		= 1 << 1,
	            FadingOut		= 1 << 2,
	            Muted			= 1 << 3,
	            Local			= 1 << 4,
	            ThreeD			= 1 << 5,
	            Paused			= 1 << 6,
	            Occludable  	= 1 << 7,
	            Occluded  		= 1 << 8,
	            Delayed			= 1 << 9,
            };            
            
            private void _SetFlag(Flags flag, bool on)
			{
				if(on)
				{
					_flags |= flag;
				}
				else
				{
					_flags &= ~flag;
				}
			}

			private bool _GetFlag(Flags flag)
			{
				return ((_flags & flag) != 0); 
			}
			
			private bool _AcquireSource()
			{
				if(!_source)
				{
					AudioCue srcCue = _audioCue;
					bool useLowpass = Occludable && !srcCue.bypassEffects ;
					useLowpass = useLowpass && _lowpassSourcePool.Count > 0;
					_source = useLowpass ? _lowpassSourcePool.Pop() : _simpleAudioSourcePool.Pop();
	
					if(_source)
					{
						if(useLowpass)
						{
							_lowpass = _source.GetComponent<AudioLowPassFilter>();
							_lowpass.enabled = false;
						}
	
						_source.volume = 1f;
						_source.pitch = 1f;
						_source.time = 0;
						_source.timeSamples = 0;
						_source.priority = srcCue.priority;
						_source.bypassEffects = srcCue.bypassEffects;
						_source.bypassReverbZones = srcCue.bypassEffects;
						_source.loop = srcCue.loops;
						_source.spread = srcCue.spread;
						_source.mute = Muted;
						_basePitch = srcCue.pitch;
						_source.panStereo = 0f;
						_source.spatialBlend = 1f;
	
						if(Local)
						{
							if(_3D)
							{
								_source.rolloffMode = AudioRolloffMode.Linear;
								_source.maxDistance = 1000000;
								_source.minDistance = _source.maxDistance - 0.001f;
							}
							else
							{
								_source.spatialBlend = 0;
							}
							
							_source.dopplerLevel = 0f;

							if((_ambience != null && _ambience.backgroundLoop == _audioCue) || (_currentMusic != null && _currentMusic == _audioCue))
							{
								_source.priority = 0;
							}
						}
						else
						{
							switch(srcCue.falloff)
							{
								case AudioCue.FalloffTypes.Logarithmic:
									_source.rolloffMode = AudioRolloffMode.Logarithmic;
									break;
								case AudioCue.FalloffTypes.Linear: default:
									_source.rolloffMode = AudioRolloffMode.Linear;
									break;
							}
							// Min must be set before Max or Unity may ignore it.
							_source.minDistance = srcCue.minDistance;
							_source.maxDistance = Mathf.Max(srcCue.maxDistance, srcCue.minDistance + 0.001f);
							_source.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;
						}
						_source.transform.position = Position;
						_source.gameObject.SetActive(true);
					}
				}
	
				return _source != null;
			}

			private void _ReleaseSource()
			{
				if (_source != null)
				{
					_source.Stop();
					_source.clip = null;
					_source.gameObject.SetActive(false);
				}
			}

			private void _ScheduleNextTest()
			{
				_nextTestTime = _currentTime + Random.Range(_audioManager.retestInterval.x, _audioManager.retestInterval.y);
			}
	
			private void _Stop(bool stopImmediately)
			{
				if(!stopImmediately && _source && _source.isPlaying && _audioCue && _audioCue.fadeOutTime > 0)
				{
					if(FadingIn)
					{
						// It's possible that we're stopped while fading in, so to keep the volume from
						// popping we simply adjust the fadeStartTime of the fade out.
						float curFade = 1 - Mathf.Clamp01((_currentTime - _fadeStartTime) / _audioCue.fadeInTime);
						_fadeStartTime = _currentTime - (curFade * _audioCue.fadeOutTime);
					}
					else
					{
						_fadeStartTime = _currentTime;
					}
					_SetFlag(Flags.FadingOut, true);
					_SetFlag(Flags.FadingIn, false);
				}
				else
				{
	                if (keepAliveTimeStamp == 0)
	                {
		                keepAliveTimeStamp = GetUnixTimeStamp();
	                }
	
	                if (GetUnixTimeStamp() >= keepAliveTimeStamp + _audioCue.keepAliveTime)
	                {
	                    _SetFlag(Flags.FadingOut, false);
	                    _ReleaseSource();
	                }
	                else
	                {
	                    _SetFlag(Flags.FadingOut, true);
	                }
				}
			}
            #endregion
        }

        // Singleton
        private static AudioManager _audioManager;

        // Instances
        private static Stack<Instance> _instancePool;
        private static Stack<AudioSource> _simpleAudioSourcePool;
        private static Stack<AudioSource> _lowpassSourcePool;
        private static Transform _sourcePoolParent;
        private static List<Instance> _activeInstances;


        // Time
        private static float _currentTime;
        
        // Ambience
        private static AmbientAudio _ambience;
        private static float _nextAmbienceOneShotTime;
        
        // Music
        private static AudioCue _currentMusic;
        private static AudioCueInstance _musicLoop;
        
        #endregion

        #region Public Interface
        
        [Flags]
        public enum OcclusionModes
        {
            Raycast = 1 << 0,
            Distance = 1 << 1,
        }
        
        [Header("General Properties")]
		[Tooltip("The maximum number of instances that can be active at once. Inaudible sounds do not count against this limit.")]
		public int maxInstances = 128;
		[Tooltip("The number of instances to allocate with lowpass effects (for occlusion and the like).")]
		public int lowpassInstances = 32;
		[Tooltip("The mixer at the top of the hierarchy. Required to play sounds.")]
		public AudioMixGroup masterGroup;
		[Tooltip("The baseline settings for any environmental audio.")]
		public AmbientAudio defaultAmbience = new();
		
		[Space(15)]
		[Header("Blending Properties")]
		[Tooltip("Should sounds close to the listener be blended into 2D (to avoid harsh stereo switching).")]
		public bool blendNearbySounds = true;
		[Tooltip("Objects close to the listener will be blended into 2D. This determines the start and end of that blend.")]
		public Vector2 nearBlendRange = new Vector2(0.25f, 0.75f);
		
		[Space(15)]
		[Header("Occlusion Properties")]
		[Tooltip("Determines what kind of logic to use for computing sound occlusion.")]
		public OcclusionModes occlusionMethod = OcclusionModes.Raycast;
		[Tooltip("The distance beyond which sounds will be considered occluded, if Distance occlusion is enabled.")]
		public float occlusionDistance = 100f;
		[Tooltip("The layers to test against when raycasting for occlusion.")]
		public LayerMask raycastLayers = Physics.DefaultRaycastLayers;
	 
		[Space(8)]
		[Tooltip("The amount by which to decrease the volume of occluded sounds.")]
		public float occlusionVolume = 0.5f;
		[Tooltip("The frequency cutoff of the lowpass filter for occluded sounds.")]
		public float occlusionCutoff = 2200;
		[Tooltip("The resonance Q of the lowpass filter for occluded sounds.")]
		public float occlusionResonanceQ = 1;

		[Space(15)]
		[Header("Advanced Properties")]
		[Tooltip("The amount of time between tests to see if looping sounds should start or stop running.")]
		public Vector2 retestInterval = new Vector2(0.5f, 1f);
		[Tooltip("The amount of buffer to give before culling distant sounds.")]
		public float cullingBuffer = 10f;
		
		public static bool Initialized => _audioManager != null;
		public static AudioManager AudioManagerInstance => _audioManager;
		public static Transform Listener => _audioManager.transform;

		public static AudioCueInstance Play(AudioCue audioCue, Vector3 position, bool loop)
		{
			return Play(audioCue, null, position, loop);
		}
		
		public static AudioCueInstance Play(AudioCue audioCue, Transform parent, Vector3 localPosition, bool loop)
		{
			if(!Initialized)
			{
				Debug.LogWarning("Cannot play sounds before AudioManager is initialized.");
				return new AudioCueInstance();
			}
			else if(_audioManager.masterGroup == null)
			{
				Debug.LogWarning("AudioManager needs a Master Group before you can play sounds.");
				return new AudioCueInstance();
			}
			else if(_activeInstances.Count >= _audioManager.maxInstances || _instancePool.Count == 0)
			{
				Debug.LogWarning("Global max audio instances exceeded.");
				return new AudioCueInstance();
			}
			else if(audioCue == null)
			{
				return new AudioCueInstance();
			}
			
			bool inRange = audioCue.IsLocal || CheckProximity(audioCue, parent, localPosition);
			loop |= audioCue.loops;
			
			if(inRange || loop)
			{
				Instance newInstance = _instancePool.Pop();
				_activeInstances.Add(newInstance);
				newInstance.Init(audioCue, parent, localPosition, loop);
				if(inRange)
				{
					newInstance.Play();
				}
				return new AudioCueInstance(newInstance, newInstance.Generation);
			}
			
			return new AudioCueInstance();
		}
		
		public static void PlayMusic(AudioCue musicCue)
		{
			if(!Initialized)
			{
				Debug.LogWarning("Cannot play music before Audio System is initialized.");
			}
			else if(musicCue != null)
			{
				if(musicCue.Is3D)
				{
					Debug.LogWarning("Music Cue " + musicCue.name + "is 3D but music should be Simple 2D.");
				}
			
				_musicLoop.Stop(false);
				_currentMusic = musicCue;
				_musicLoop = Play(_currentMusic, Listener, Vector3.zero, true);
			}
		}
		
		public static void StopMusic(bool stopImmediate)
		{
			if(Initialized)
			{
				_musicLoop.Stop(stopImmediate);
				_currentMusic = null;
			}
		}
		
		public static void SetAmbience(AmbientAudio ambience)
		{
			if(!Initialized)
			{
				Debug.LogWarning("Cannot activate an ambience before audio system is initialzied.");
			}
			else if(ambience != null)
			{
				_ambience = ambience;
			}
		}
		
		public static void ClearAmbience()
		{
			if(Initialized && _ambience != null)
			{
				_ambience = null;
			}
		}
		
		public static void SetMixerVolume(string groupName, float volume)
		{
			if(!Initialized)
			{
				Debug.LogWarning("Cannot activate an ambience before audio system is initialized.");
			}
			else if(!string.IsNullOrEmpty(groupName))
			{
				SetMixerVolume(FindGroup(_audioManager.masterGroup, groupName), volume);
			}
		}
		
		public static void SetMixerVolume(AudioMixGroup group, float volume)
		{
			if(!Initialized)
			{
				Debug.LogWarning("Cannot set bus volume before Audio System is initialized.");			
			}
			else if(group)
			{
				group.UserVolume = volume;
			}
		}
		
		public static void MuteMixer(string groupName, bool mute)
		{
			if(!Initialized)
			{
				Debug.LogWarning("Cannot mute bus before Audio System is initialized.");			
			}
			else if(!string.IsNullOrEmpty(groupName))
			{
				MuteMixer(FindGroup(_audioManager.masterGroup, groupName), mute);
			}
		}
		
		public static void MuteMixer(AudioMixGroup group, bool mute)
		{
			if(!Initialized)
			{
				Debug.LogWarning("Cannot mute bus before Audio System is initialized.");			
			}
			else if(group)
			{
				group.Muted = mute;
			}
		}
		
		public static void PauseMixer(AudioMixGroup group, bool paused)
		{
			if(!Initialized)
			{
				Debug.LogWarning("Cannot pause bus before Audio System is initialized.");			
			}
			else if(group)
			{
				int numInstances = _activeInstances.Count;
				for(int instanceIndex = 0; instanceIndex < numInstances; ++instanceIndex)
				{
					Instance instance = _activeInstances[instanceIndex];
					if(group.IsAncestorOf(instance.Group))
					{
						instance.Pause = paused;
					}
				}
			}
		}
		
		public static bool IsOccluded(Vector3 worldSpacePosition, OcclusionModes occlusionFlags)
		{
			bool occluded = false;
			Vector3 listenerPosition = Listener.position;
			Vector3 toListener = listenerPosition - worldSpacePosition;
			float distanceToListenerSqr = toListener.sqrMagnitude;

			if(((occlusionFlags & OcclusionModes.Distance) != 0))
			{
				occluded = distanceToListenerSqr >= (_audioManager.occlusionDistance * _audioManager.occlusionDistance); 
			}
			else if(((occlusionFlags & OcclusionModes.Raycast) != 0))
			{
				float distanceToListener = Mathf.Sqrt(distanceToListenerSqr);
				bool hitSomething = Physics.Raycast(worldSpacePosition, toListener, out var hit, distanceToListener, _audioManager.raycastLayers); 
				occluded = hitSomething && hit.transform != Listener;
			}
			return occluded;
		}
		#endregion

		#region Unity Methods

		private void OnEnable()
        {
            if(_audioManager && _audioManager != this)
            {
                DestroyImmediate(this);
            }
            else if(_audioManager == null)
            {
                _audioManager = this;
                
                _instancePool = new Stack<Instance>(maxInstances);
                for(int instanceIndex = 0; instanceIndex < maxInstances; ++instanceIndex)
                {
	                _instancePool.Push(new Instance());
                }
                
                int numSimpleSources = Mathf.Max(0, maxInstances - lowpassInstances);
                int numLowpassInstances = maxInstances - numSimpleSources;
                _simpleAudioSourcePool = new Stack<AudioSource>(numSimpleSources);
                _lowpassSourcePool =  new Stack<AudioSource>(numLowpassInstances);
                
                HideFlags poolFlags = HideFlags.HideAndDontSave;
                GameObject sourcePoolParentObject = new GameObject("SourcePool");
                sourcePoolParentObject.hideFlags = poolFlags;
                _sourcePoolParent = sourcePoolParentObject.transform;
                
                for(int instanceIndex = 0; instanceIndex < numSimpleSources; ++instanceIndex)
                {
	                GameObject newSourceObject = new GameObject("SimpleInstance" + instanceIndex);
	                newSourceObject.hideFlags = poolFlags;
	                newSourceObject.transform.parent = _sourcePoolParent.transform;
	                AudioSource newSource = newSourceObject.AddComponent<AudioSource>();
	                newSource.playOnAwake = false;
	                newSourceObject.SetActive(false);
	                _simpleAudioSourcePool.Push(newSource);
                }
                
                for(int instanceIndex = 0; instanceIndex < numLowpassInstances; ++instanceIndex)
                {
	                GameObject newSourceObject = new GameObject("LowpassInstance" + instanceIndex);
	                newSourceObject.hideFlags = poolFlags;
	                newSourceObject.transform.parent = _sourcePoolParent.transform;
	                AudioSource newSource = newSourceObject.AddComponent<AudioSource>();
	                newSource.playOnAwake = false;
	                AudioLowPassFilter newLowpass = newSourceObject.AddComponent<AudioLowPassFilter>();
	                newLowpass.enabled = false;
	                newSourceObject.SetActive(false);
	                _lowpassSourcePool.Push(newSource);
                }
                
                _activeInstances = new List<Instance>(maxInstances);


                _UpdateTime();
                if(masterGroup != null)
                {
	                masterGroup.ResetUserVolume();
	                UpdateMixerPitchVolume(masterGroup, 1f, 1f);
                }
                else
                {
	                Debug.LogWarning("No MasterGroup assigned. Game sounds will not play.");
                }
            }
	    }
		
		private void OnDisable()
		{
			if(_audioManager == this)
			{
				int numActive = _activeInstances.Count;
				for(int instanceIndex = 0; instanceIndex < numActive; ++instanceIndex)
				{
					Instance instance = _activeInstances[instanceIndex];
					instance.Stop(true);
				}

				if(_sourcePoolParent)
				{
					Destroy(_sourcePoolParent.gameObject);
					_sourcePoolParent = null;
				}
				
				_audioManager = null;
				_activeInstances = null;
				_instancePool = null;
				_simpleAudioSourcePool = null;
				_lowpassSourcePool = null;

				_currentTime = 0;
				
				_ambience = null;
				_nextAmbienceOneShotTime = 0;
				_currentMusic = null;
			}
		}
		
		void LateUpdate()
		{
			if(_audioManager == this && !AudioListener.pause && masterGroup)
			{
				float deltaTime = _UpdateTime();
				UpdateMixerPitchVolume(masterGroup, 1f, 1f);
				UpdateAmbience();
				
				// Update all of the instances.
				int numActiveInstances = _activeInstances.Count;
				int instanceIndex = 0;
				while(instanceIndex < numActiveInstances)
				{
					Instance instance = _activeInstances[instanceIndex];
					instance.Update(deltaTime, false);
					if(!instance.Active && !instance.FadingOut)
					{
	                    // Any instances that are inactive after update are recycled.
	                    if (instance.keepAliveTimeStamp == 0)
	                    {
	                        instance.keepAliveTimeStamp = GetUnixTimeStamp();
	                    }

	                    if (GetUnixTimeStamp() >= instance.keepAliveTimeStamp + instance.Cue.keepAliveTime)
	                    {
	                        instance.UnInit();
	                        _activeInstances.RemoveAt(instanceIndex);
	                        _instancePool.Push(instance);
	                        --numActiveInstances;
	                    }
	                    else
	                    {
		                    ++instanceIndex;
	                    }
					}
					else
					{
						++instanceIndex;
					}
				}
			}
		}

		#endregion
		
		#region Private Methods
		
		private static bool CheckProximity(AudioCue audioCue, Transform parent, Vector3 position)
		{
			if (parent)
			{
				position = parent.localToWorldMatrix.MultiplyPoint3x4(position);
			}

			float bufferDistance = audioCue.maxDistance + _audioManager.cullingBuffer;
			return Vector3.SqrMagnitude(position - Listener.position) <= (bufferDistance * bufferDistance);
		}
		
		private static double GetUnixTimeStamp()
		{
			return (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
		}
		
		private static float _UpdateTime()
		{
			float newTime = (float)AudioSettings.dspTime;
			float deltaTime = (newTime - _currentTime);
			_currentTime = newTime;
			return deltaTime;
		}
		
		private static void UpdateMixerPitchVolume(AudioMixGroup group, float effectiveVolume, float effectivePitch)
		{
			if(group)
			{
				group.EffectiveVolume = effectiveVolume;
				group.EffectivePitch = effectivePitch;
				int numChildren = group.Children.Count;
				for(int childIndex = 0; childIndex < numChildren; ++childIndex)
				{
					UpdateMixerPitchVolume(group.Children[childIndex], group.EffectiveVolume, group.EffectivePitch);
				}
			}
		}


		private static void UpdateAmbience()
		{
			if(_ambience != null)
			{
				if(_ambience.oneShots.Count > 0)
				{
					_nextAmbienceOneShotTime = _currentTime + Random.Range(_ambience.oneShotInterval.x, _ambience.oneShotInterval.y);
				}

				if(_ambience.backgroundLoop)
				{
					Play(_ambience.backgroundLoop, Listener, Vector3.zero, true);
				}
				
				int numOneShots = _ambience.oneShots.Count;
				if(numOneShots > 0 && _currentTime >= _nextAmbienceOneShotTime)
				{
					AudioCue nextAmbienceOneShot = _ambience.oneShots[Random.Range(0, numOneShots)];
					if(nextAmbienceOneShot != null)
					{
						if(nextAmbienceOneShot.loops)
						{
							Debug.LogWarning("Cannot play ambient one shot " + nextAmbienceOneShot.name + ". It is set to loop.");
						}
						else
						{
							if(!nextAmbienceOneShot.IsLocal)
							{
								Debug.LogWarning("Ambient one shot " + nextAmbienceOneShot.name + "should be 2D");
							}
							Play(nextAmbienceOneShot, Listener, Random.onUnitSphere, false);
						}
					}
					_nextAmbienceOneShotTime = _currentTime + Random.Range(_ambience.oneShotInterval.x, _ambience.oneShotInterval.y);
				}
			}
		}


		private static AudioMixGroup FindGroup(AudioMixGroup group, string groupName)
		{
			if(group)
			{
				if(group.name == groupName)
				{
					return group;
				}
				else
				{
					int numChildren = group.Children.Count;
					for(int childIndex = 0; childIndex < numChildren; ++childIndex)
					{
						AudioMixGroup foundBus = FindGroup(group.Children[childIndex], groupName);
						if(foundBus)
						{
							return foundBus;
						}
					}
				}
			}
			return null;
		}
		
		#endregion
    }
}
