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
    [DefaultExecutionOrder(4)]
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
        private float _masterVolume = 1.0f;
        
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
            
            if(GameManager.Instance.testMode || SceneManager.GetActiveScene().name == "MainMenu") StartSounds();
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
                DontDestroyOnLoad(audioSource);
                _activeSoundInstanceSources.Add(audioSource, sound);
                destroy = true;
            }
            
            audioSource.outputAudioMixerGroup = sound.mixerGroup;
            audioSource.clip = sound.clip;
            audioSource.volume = sound.volumeScale;
            audioSource.loop = sound.loop;
            audioSource.Play();
            
            if (destroy && !sound.loop) StartCoroutine(DestroySoundSource(audioSource, audioSource.clip.length+0.5f));
        }
        
        public void PlayRealtime(Sound sound)
        {
            if (sound == null || sound.loop) return;
            
            var audioSource = new GameObject(sound.name + " source").AddComponent<AudioSource>();
            DontDestroyOnLoad(audioSource);
            
            audioSource.outputAudioMixerGroup = sound.mixerGroup;
            audioSource.clip = sound.clip;
            audioSource.volume = sound.volumeScale;
            audioSource.Play();
            
            StartCoroutine(DestroySoundSource(audioSource, audioSource.clip.length+0.5f));
        }
        
        public void PlayOneShot(Sound sound, AudioSource audioSource)
        {
            if (sound == null || sound.loop) return;
            
            audioSource.PlayOneShot(sound.clip, sound.volumeScale);
        }

        public void SetupSound(AudioSource audioSource, Sound sound)
        {
            audioSource.outputAudioMixerGroup = sound.mixerGroup;
            audioSource.clip = sound.clip;
            audioSource.volume = sound.volumeScale;
            audioSource.loop = sound.loop;
        }
        
        public void StartSounds()
        {
            StartCoroutine(SoundUtils.StartFade(mainMixer, "masterVolume", 1.0f, _masterVolume));
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
            SilenceAmbience();
            mainMixer.SetFloat("masterVolume", -80.0f);
            
            _activeSoundInstanceSources.Clear();
            _activeSoundEntitySources.Clear();
        }
        
        public void FadeInAmbience(float fadeTime = 1.0f)
        {
            StartCoroutine(SoundUtils.StartFade(mainMixer, _ambMixerNames[_currentAmbIndex], fadeTime, 0.0f));
        }
        
        public void FadeOutAmbience(float fadeTime = 1.0f)
        {
            StartCoroutine(SoundUtils.StartFade(mainMixer, _ambMixerNames[_currentAmbIndex], fadeTime, -80.0f));
        }

        public void SilenceAmbience()
        {
            mainMixer.SetFloat(_ambMixerNames[_currentAmbIndex], -80.0f);
        }
        
        public void FadeToNextAmbience(Sound nextAmb, float fadeTime = 1.0f)
        {
            ambienceSources[1 - _currentAmbIndex].clip = nextAmb.clip;
            ambienceSources[1 - _currentAmbIndex].volume = nextAmb.volumeScale;
            ambienceSources[1 - _currentAmbIndex].loop = nextAmb.loop;
            ambienceSources[1 - _currentAmbIndex].Play();
            
            StartCoroutine(SoundUtils.StartFade(mainMixer, _ambMixerNames[_currentAmbIndex], fadeTime, -80.0f));
            StartCoroutine(SoundUtils.StartFade(mainMixer, _ambMixerNames[1 - (_currentAmbIndex)], fadeTime, 1.0f));
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
            
            Invoke(nameof(StingReady), sound.clip.length/2);
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

        public void LoadMasterVolume(float volume)
        {
            Debug.Log("Loading master volume: " + volume);
            _masterVolume = volume;
        }
        
        public void SetMasterVolume(float volume)
        {
            mainMixer.SetFloat("masterVolume", Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1.4f)) * 20);
        }

        public void SetMusicVolume(float volume)
        {
            mainMixer.SetFloat("ambienceVolume", Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1.4f)) * 20);
        }
        
        public void SetVoicesVolume(float volume)
        {
            mainMixer.SetFloat("voicesVolume", Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1.4f)) * 20);
        }

        public void SetSfxVolume(float volume)
        {
            mainMixer.SetFloat("sfxVolume", Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1.4f)) * 20);
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
