/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System.Collections.Generic;
using System.Linq;
using Runtime.Managers;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.SoundSystem
{
	public class SoundManager : MonoBehaviour
	{
        [Header("MIX GROUPS")]
        [SerializeField] private SoundMixGroup masterMixGroup;
        [SerializeField] private SoundMixGroup musicMixGroup;
        [SerializeField] private SoundMixGroup sfxMixGroup;

        private Sound _activeMusic;
        
        private AudioListener _audioListener;
        private readonly Dictionary<AudioSource, Sound> _activeAudioSources = new();
        
        private void Awake()
        {
            if (masterMixGroup == null)
            {
                Debug.LogError("Master Mix Group is null");
            }
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            _audioListener = FindObjectOfType(typeof(AudioListener)) as AudioListener;
            SetMasterVolume(masterMixGroup.volume);
        }

        private void LateUpdate()
        {
            var audioSourcesToRemove = (from audioSource in _activeAudioSources where !audioSource.Key.isPlaying select audioSource.Key).ToList();
            foreach (var audioSource in audioSourcesToRemove)
            {
                _activeAudioSources.Remove(audioSource);
                Destroy(audioSource.gameObject);
            }
        }

        public void Play(Sound sound, Transform origin = null, bool music = false)
        {
            if (sound == null) return;

            if (!music)
            {
                var audioSource = new GameObject(sound.name + " source").AddComponent<AudioSource>();
                audioSource.transform.position = origin == null ? _audioListener.transform.position : origin.position;
                DontDestroyOnLoad(audioSource);
                _activeAudioSources.Add(audioSource, sound);
                
                audioSource.clip = sound.clip;
                audioSource.volume = sound.volume;
                audioSource.loop = sound.loop;
                audioSource.spatialBlend = origin == null ? 0 : 1;
                audioSource.Play();
            }
            else
            {
                if (_activeMusic != null)
                {
                    
                }
                _activeMusic = sound;
                
                var audioSource = FindObjectOfType<LevelManager>().gameObject.GetComponent<AudioSource>();
                audioSource.clip = sound.clip;
                // audioSource.volume = sound.volume;
                audioSource.loop = sound.loop;
                audioSource.spatialBlend = 0;
                audioSource.Play();
            }
        }
        
        public void StopAll()
        {
            foreach (var audioSource in _activeAudioSources)
            {
                audioSource.Key.Stop();
                Destroy(audioSource.Key.gameObject);
            }
            _activeAudioSources.Clear();
        }
        
        public void Stop(Sound sound)
        {
            var audioSourcesToRemove = (from audioSource in _activeAudioSources where audioSource.Value == sound select audioSource.Key).ToList();
            foreach (var audioSource in audioSourcesToRemove)
            {
                _activeAudioSources.Remove(audioSource);
                audioSource.Stop();
                Destroy(audioSource.gameObject);
            }
        }
        
        public void PauseAll()
        {
            foreach (var audioSource in _activeAudioSources)
            {
                audioSource.Key.Pause();
            }
        }

        public void ResumeAll()
        {
            foreach (var audioSource in _activeAudioSources)
            {
                audioSource.Key.UnPause();
            }
        }

        public void SetMasterVolume(float volume)
        {
            masterMixGroup.SetVolume(volume);
        }

        public void SetMusicVolume(float volume)
        {
            musicMixGroup.SetVolume(volume);
        }

        public void SetSfxVolume(float volume)
        {
            sfxMixGroup.SetVolume(volume);
        }
		
	}
}
