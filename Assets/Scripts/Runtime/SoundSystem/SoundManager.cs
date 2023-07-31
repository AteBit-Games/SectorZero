/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections;
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
        private readonly Dictionary<AudioSource, Sound> _activeSoundInstanceSources = new();
        private readonly List<ISoundEntity> _activeSoundEntitySources = new();
        private readonly Dictionary<AudioSource, Sound> _activeMusicSources = new();

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
            _activeSoundEntitySources.Clear();
            _activeMusicSources.Clear();
            
            var soundSources = SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<ISoundEntity>()).ToList();
            foreach (var soundSource in soundSources)
            {
                _activeSoundEntitySources.Add(soundSource);
            }
            SetSfxVolume(sfxMixGroup.volume);

            _audioListener = FindObjectOfType(typeof(AudioListener)) as AudioListener;
        }

        public void Play(Sound sound, Transform origin = null, bool music = false)
        {
            if (sound == null) return;

            if (!music)
            {
                var audioSource = new GameObject(sound.name + " source").AddComponent<AudioSource>();
                audioSource.transform.position = origin == null ? _audioListener.transform.position : origin.position;
                DontDestroyOnLoad(audioSource);
                _activeSoundInstanceSources.Add(audioSource, sound);
                
                audioSource.clip = sound.clip;
                audioSource.volume = sound.volume;
                audioSource.loop = sound.loop;
                audioSource.spatialBlend = origin == null ? 0 : 1;
                audioSource.Play();
                
                if (!sound.loop) StartCoroutine(DestroySoundSource(audioSource, audioSource.clip.length+0.5f));
            }
            else
            {
                if (_activeMusic != null && _activeMusicSources.Any())
                {
                    _activeMusicSources.First().Key.Stop();
                }
                _activeMusic = sound;
                _activeMusicSources.Clear();
                
                var audioSource = FindObjectOfType<LevelManager>().gameObject.GetComponent<AudioSource>();
                audioSource.clip = sound.clip;
                audioSource.volume = sound.volume;
                audioSource.loop = sound.loop;
                audioSource.spatialBlend = 0;
                audioSource.Play();
                _activeMusicSources.Add(audioSource, sound.CreateInstance());
            }
        }
        
        public void StopAll()
        {
            foreach (var audioSource in _activeSoundInstanceSources)
            {
                audioSource.Key.Stop();
                Destroy(audioSource.Key.gameObject);
            }
            _activeSoundInstanceSources.Clear();
            
            foreach (var audioSource in _activeMusicSources)
            {
                audioSource.Key.Stop();
                Destroy(audioSource.Key.gameObject);
            }
            _activeMusicSources.Clear();
        }

        public void PauseAll()
        {
            foreach (var audioSource in _activeSoundInstanceSources)
            {
                audioSource.Key.Pause();
            }
            
            foreach (var audioSource in _activeMusicSources)
            {
                audioSource.Key.Pause();
            }
            
            foreach (var audioSource in _activeSoundEntitySources)
            {
                audioSource.AudioSource.Pause();
            }
        }

        public void ResumeAll()
        {
            foreach (var audioSource in _activeSoundInstanceSources)
            {
                audioSource.Key.UnPause();
            }
            
            foreach (var audioSource in _activeMusicSources)
            {
                audioSource.Key.UnPause();
            }
            
            foreach (var audioSource in _activeSoundEntitySources)
            {
                audioSource.AudioSource.UnPause();
            }
        }

        public void SetMasterVolume(float volume)
        {
            masterMixGroup.SetVolume(volume);
        }

        public void SetMusicVolume(float volume)
        {
            musicMixGroup.SetVolume(volume);
            foreach (var musicSource in _activeMusicSources.Where(musicSource => musicSource.Key != null))
            {
                musicSource.Key.volume = volume * musicSource.Value.volumeScale;
            }
        }

        public void SetSfxVolume(float volume)
        {
            sfxMixGroup.SetVolume(volume);
            foreach (var soundSource in _activeSoundInstanceSources)
            {
                soundSource.Key.volume = volume * soundSource.Value.volumeScale;
            }
            
            foreach (var soundSource in _activeSoundEntitySources)
            {
                if(soundSource.AudioSource == null) continue;
                soundSource.Volume = volume * soundSource.VolumeScale;
                soundSource.AudioSource.volume = soundSource.Volume;
            }
        }

        private IEnumerator DestroySoundSource(AudioSource audioSource, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            if(audioSource == null) yield break;
            _activeSoundInstanceSources.Remove(audioSource);
            Destroy(audioSource.gameObject);
        }

        public void ResetSystem()
        {
            StopAll();
            _activeSoundInstanceSources.Clear();
            _activeMusicSources.Clear();
            _activeSoundEntitySources.Clear();
        }
        
        public float SfxVolume()
        {
            return sfxMixGroup.volume;
        }
    }
}
