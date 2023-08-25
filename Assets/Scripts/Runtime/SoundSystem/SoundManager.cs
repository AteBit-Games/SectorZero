/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Managers;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Runtime.SoundSystem
{
    [DefaultExecutionOrder(1)]
	public class SoundManager : MonoBehaviour
	{
        [Header("MIX GROUPS")]
        [SerializeField] public AudioMixer mainMixer;
        
        [Header("AMBIENCE SETTINGS")]
        [SerializeField] public List<AudioSource> ambienceSources;
        [SerializeField] public AudioSource stingSource;
        
        private readonly Dictionary<AudioSource, Sound> _activeSoundInstanceSources = new();
        private readonly List<ISoundEntity> _activeSoundEntitySources = new();

        private int _currentAmbIndex;
        private readonly string[] _ambMixerNames = {"mainAmbA", "mainAmbB"};
        private bool _isBusy;
        
        //========================= Unity Events =========================//

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            _currentAmbIndex = 0;
            _isBusy = false;
        }

        private void Start()
        {
            ambienceSources[0].Play();
            ambienceSources[1].Play();
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            var soundSources = SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<ISoundEntity>()).ToList();
            foreach (var soundSource in soundSources)
            {
                _activeSoundEntitySources.Add(soundSource);
            }
        }
        
        //========================= Public Methods =========================//

        public void Play(Sound sound, AudioSource audioSource = null)
        {
            if (sound == null) return;
            
            var destroy = false;
            if (audioSource == null)
            {
                audioSource = new GameObject(sound.name + " source").AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = sound.mixerGroup;
                DontDestroyOnLoad(audioSource);
                _activeSoundInstanceSources.Add(audioSource, sound);
                destroy = true;
            }
            
            audioSource.clip = sound.clip;
            audioSource.volume = sound.volumeScale;
            audioSource.loop = sound.loop;
            audioSource.Play();
            
            if (destroy && !sound.loop )StartCoroutine(DestroySoundSource(audioSource, audioSource.clip.length+0.5f));
        }
        
        public void StopAll()
        {
            foreach (var audioSource in _activeSoundInstanceSources)
            {
                audioSource.Key.Stop();
                Destroy(audioSource.Key.gameObject);
            }
            _activeSoundInstanceSources.Clear();
        }

        public void PauseAll()
        {
            foreach (var audioSource in _activeSoundInstanceSources)
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

            foreach (var audioSource in _activeSoundEntitySources)
            {
                audioSource.AudioSource.UnPause();
            }
        }
        
        public void ResetSystem()
        {
            StopAll();
            _activeSoundInstanceSources.Clear();
            _activeSoundEntitySources.Clear();
        }
        
        public void FadeInAmbience(float fadeTime = 1.0f)
        {
            StartCoroutine(SoundUtils.StartFade(GameManager.Instance.SoundSystem.mainMixer, _ambMixerNames[_currentAmbIndex], fadeTime, 0.0f));
        }
        
        public void FadeOutAmbience(float fadeTime = 1.0f)
        {
            StartCoroutine(SoundUtils.StartFade(GameManager.Instance.SoundSystem.mainMixer, _ambMixerNames[_currentAmbIndex], fadeTime, -80.0f));
        }

        public void SilenceAmbience()
        {
            GameManager.Instance.SoundSystem.mainMixer.SetFloat(_ambMixerNames[_currentAmbIndex], -80.0f);
        }
        
        public void FadeToNextAmbience(Sound nextAmb, float fadeTime = 1.0f)
        {
            ambienceSources[1 - _currentAmbIndex].clip = nextAmb.clip;
            ambienceSources[1 - _currentAmbIndex].volume = nextAmb.volumeScale;
            ambienceSources[1 - _currentAmbIndex].loop = nextAmb.loop;
            ambienceSources[1 - _currentAmbIndex].Play();
            StartCoroutine(SoundUtils.StartFade(GameManager.Instance.SoundSystem.mainMixer, _ambMixerNames[_currentAmbIndex], fadeTime, -80.0f));
            StartCoroutine(SoundUtils.StartFade(GameManager.Instance.SoundSystem.mainMixer, _ambMixerNames[1 - (_currentAmbIndex)], fadeTime, 1.0f));
            _currentAmbIndex = 1 - _currentAmbIndex;
        }

        public void FastSwapAmbience()
        {
            _currentAmbIndex = 1 - _currentAmbIndex;
            ambienceSources[_currentAmbIndex].Play();
            ambienceSources[1 - _currentAmbIndex].Stop();
        }

        public void PlaySting(Sound sound)
        {
            if(_isBusy) return;
            _isBusy = true;
            
            stingSource.volume = sound.volumeScale;
            stingSource.loop = false;
            stingSource.PlayOneShot(sound.clip);
            
            Invoke(nameof(StingReady), sound.clip.length);
        }

        //========================= Private Methods =========================//
        
        private void StingReady()
        {
            _isBusy = false;
        }
        
        //========================= Setters =========================//

        public void SetAmbienceClip(Sound sound)
        {
            ambienceSources[_currentAmbIndex].clip = sound.clip;
        }
        
        public void SetNextAmbience(Sound sound)
        {
            ambienceSources[1 - _currentAmbIndex].clip = sound.clip;
        }
        
        public void SetMasterVolume(float volume)
        {
            mainMixer.SetFloat("masterVolume", Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f))* 20);
        }

        public void SetMusicVolume(float volume)
        {
            mainMixer.SetFloat("ambienceVolume", Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f)) * 20);
        }

        public void SetSfxVolume(float volume)
        {
            mainMixer.SetFloat("sfxVolume", Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f)) * 20);
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
        
        //========================= Coroutines =========================//
        
        private IEnumerator DestroySoundSource(AudioSource audioSource, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            if(audioSource == null) yield break;
            _activeSoundInstanceSources.Remove(audioSource);
            Destroy(audioSource.gameObject);
        }
    }
}
