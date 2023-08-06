using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [SerializeField] public AudioMixer _audioMixer;
    [SerializeField] public AudioSource _currentAmbiance;
    [SerializeField] public AudioSource _nextAmbiance;

    [SerializeField] public List<AudioClip> _audioClips;

    private int _currentAmbianceIndex;
    private bool _isCurrentAmbiance = true;

    private void Start()
    {
        _currentAmbiance.loop = true;
        _nextAmbiance.loop = true;

        if (_audioClips.Count > 0)
        {
            _currentAmbiance.clip = _audioClips[0];
            _currentAmbianceIndex = 0;
            _currentAmbiance.Play();
        }

        if (_audioClips.Count > 1)
        {
            _nextAmbiance.clip = _audioClips[1];
            _currentAmbianceIndex = 1;
            _nextAmbiance.Play();
        }

        Invoke(nameof(FadeToNext), 5f);
        
        Invoke(nameof(FadeToNext), 10f);
        
        Invoke(nameof(FadeToNext), 15f);
    }
    
    public void Play()
    {
        _currentAmbiance.Play();
    }
    
    public void Stop()
    {
        _currentAmbiance.Stop();
    }
    
    public void Mute()
    {
        _currentAmbiance.mute = true;
    }
    
    public void Unmute()
    {
        _currentAmbiance.mute = false;
    }

    public void FadeOut()
    {
        StartCoroutine(FadeMixerGroup.StartFade(_audioMixer, "currentAmbiance", 1.0f, -80.0f));
    }
    
    public void FadeIn(float fadeTime = 1.0f)
    {
        StartCoroutine(FadeMixerGroup.StartFade(_audioMixer, "currentAmbiance", 1.0f, 0.0f));
    }
    
    public void FadeToNext()
    {
        
        _currentAmbiance.outputAudioMixerGroup = _audioMixer.FindMatchingGroups("CurrentAmbiance")[0];
        _nextAmbiance.outputAudioMixerGroup = _audioMixer.FindMatchingGroups("NextAmbiance")[0];
        
        if(_isCurrentAmbiance)
        {
            StartCoroutine(FadeMixerGroup.StartFade(_audioMixer, "currentAmbiance", 1.0f, -80.0f));
            StartCoroutine(FadeMixerGroup.StartFade(_audioMixer, "nextAmbiance", 1.0f, 1.0f));
        }
        else
        {
            StartCoroutine(FadeMixerGroup.StartFade(_audioMixer, "currentAmbiance", 1.0f, 1.0f));
            StartCoroutine(FadeMixerGroup.StartFade(_audioMixer, "nextAmbiance", 1.0f, -80.0f));
        }
        
        _isCurrentAmbiance = !_isCurrentAmbiance;
        
        Invoke(nameof(readyNext), 1f);
    }
    
    public void FastSwap()
    {
        _audioMixer.SetFloat("currentAmbiance", 0.0f);
        _audioMixer.SetFloat("nextAmbiance", -80.0f);
    }
    
    public void setAmbiance(AudioSource ambiance)
    {
        _currentAmbiance = ambiance;
    }
    
    public void setNextAmbiance(AudioSource ambiance)
    {
        _nextAmbiance = ambiance;
    }
    
    public void readyNext()
    {
        
        _currentAmbianceIndex = (_currentAmbianceIndex + 1) % _audioClips.Count;
        if (_isCurrentAmbiance)
        {
            _nextAmbiance.clip = _audioClips[_currentAmbianceIndex];
            Debug.Log(_nextAmbiance.clip);
            _nextAmbiance.Play();
        }
        else
        {
            _currentAmbiance.clip = _audioClips[_currentAmbianceIndex];
            Debug.Log(_currentAmbiance.clip);
            _currentAmbiance.Play();
        }
    }
    

}