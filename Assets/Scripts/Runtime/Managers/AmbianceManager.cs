using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Runtime.Managers
{
    public class AmbienceManager : MonoBehaviour
    {
        [SerializeField] public List<AudioSource> audioSources;

        private int _currentAmbIndex;
        private readonly string[] _ambMixerNames = {"mainAmbA", "mainAmbB"};
        private bool _isBusy;

        private void Awake()
        {
            _currentAmbIndex = 0;
            _isBusy = false;
        }
        
        private void Start()
        {
            audioSources[0].Play();
            audioSources[1].Play();
        }
        
        public void FadeIn(float fadeTime = 1.0f)
        {
            StartCoroutine(SoundUtils.StartFade(GameManager.Instance.SoundSystem.mainMixer, _ambMixerNames[_currentAmbIndex], fadeTime, 0.0f));
        }
        
        public void FadeOut(float fadeTime = 1.0f)
        {
            StartCoroutine(SoundUtils.StartFade(GameManager.Instance.SoundSystem.mainMixer, _ambMixerNames[_currentAmbIndex], fadeTime, -80.0f));
        }

        public void FadeToNext(AudioClip nextAmb, float fadeTime = 1.0f)
        {
            Debug.Log("FadeToNext");
            Debug.Log("currentAmbIndex: " + _currentAmbIndex);
            audioSources[1 - _currentAmbIndex].clip = nextAmb;
            audioSources[1 - _currentAmbIndex].Play();
            Debug.Log(nextAmb.name);
            StartCoroutine(SoundUtils.StartFade(GameManager.Instance.SoundSystem.mainMixer, _ambMixerNames[_currentAmbIndex], fadeTime, -80.0f));
            StartCoroutine(SoundUtils.StartFade(GameManager.Instance.SoundSystem.mainMixer, _ambMixerNames[1 - (_currentAmbIndex)], fadeTime, 1.0f));
            _currentAmbIndex = 1 - _currentAmbIndex;
        }

        public void FastSwap()
        {
            _currentAmbIndex = 1 - _currentAmbIndex;
            audioSources[_currentAmbIndex].Play();
            audioSources[1 - _currentAmbIndex].Stop();
        }

        public void SetClip(AudioClip clip)
        {
            audioSources[_currentAmbIndex].clip = clip;
        }
        
        public void SetNextClip(AudioClip clip)
        {
            audioSources[1 - _currentAmbIndex].clip = clip;
        }
        
        public void PlaySting(AudioClip clip)
        {
            if(_isBusy) return;
            _isBusy = true;
            audioSources[_currentAmbIndex].PlayOneShot(clip);
            Invoke(nameof(StingReady), clip.length);
        }
        
        public void StingReady()
        {
            _isBusy = false;
        }

        public void Play()
        {
            audioSources[_currentAmbIndex].Play();
        }

        public void Stop()
        {
            audioSources[_currentAmbIndex].Stop();
        }

        public void Mute()
        {
            audioSources[_currentAmbIndex].mute = true;
        }

        public void Unmute()
        {
            audioSources[_currentAmbIndex].mute = false;
        }
    }
}