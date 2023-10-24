/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Runtime.Managers;

namespace Runtime.SoundSystem
{
    [DefaultExecutionOrder(6)]
	public class HeartBeatSystem : MonoBehaviour
	{
        [SerializeField] private int normalRate = 60;
        [SerializeField] private int maxRate = 140;
        [SerializeField] private int recoverySpeed = 1;
        [SerializeField] private int recoveryDelay = 10;
        [SerializeField] private Sound beat1;
        [SerializeField] private Sound beat2;
        [SerializeField] private AnimationCurve volumeCurve;
        
        //----- Private Variables -----//
        private List<AudioSource> _audioSources;
        private Coroutine _heartbeatCoroutine;
        private Coroutine _heartBeatRecoveryCoroutine;
        private int _currentRate;
        
        public void Enable() 
        { 
            StartHeartbeat(); 
        }
        
        public void Disable()
        {
            StopRecovery();
            StopHeartbeat();
        }
        
        public void SetHeartRateImmediately(int rate)
        {
            if (_currentRate == rate) return;

            StopRecovery();
            SetCurrentRate(rate);
        }
        
        private void SetCurrentRate(int rate) 
        {
            rate = Mathf.Clamp(rate, normalRate, maxRate);
            
            _currentRate = rate;
            UpdateHeartBeatVolume();
        }
        
        private IEnumerator HeartBeatCoroutine() 
        {
            while (true) 
            {
                if (_currentRate > normalRate && _heartBeatRecoveryCoroutine == null)
                    _heartBeatRecoveryCoroutine = StartCoroutine(HeartBeatRecoveryCoroutine());

                PlayFirstBeatSound();
                yield return new WaitForSeconds(GetSecondBeatDelay());
                PlaySecondBeatSound();
                yield return new WaitForSeconds(GetFirstBeatDelay());
            }
        }
        
        private IEnumerator HeartBeatRecoveryCoroutine()
        {
            float t = 0;
            while (t < recoveryDelay)
            {
                t += Time.deltaTime;
                yield return null;
            }

            while (_currentRate - recoverySpeed >= normalRate)
            {
                yield return new WaitForSeconds(1);
                SetCurrentRate(_currentRate - recoverySpeed);
            }

            if (_currentRate != normalRate)
            {
                yield return new WaitForSeconds(1);
                SetCurrentRate(normalRate);
            }
            _heartBeatRecoveryCoroutine = null;
        }
        
        private void UpdateHeartBeatVolume()
        {
            var ratePercent = (_currentRate - normalRate) / (float)(maxRate - normalRate);
            var volume = Mathf.Clamp(volumeCurve.Evaluate(ratePercent), 0, 1);

            foreach (var source in _audioSources) source.volume = volume;
        }
        
        private void StopRecovery() 
        {
            if (_heartBeatRecoveryCoroutine == null) return;
            StopCoroutine(_heartBeatRecoveryCoroutine);
            _heartBeatRecoveryCoroutine = null;
        }
        
        private void StopHeartbeat()
        {
            if (_heartbeatCoroutine == null) return;
            StopCoroutine(_heartbeatCoroutine);
            _heartbeatCoroutine = null;
        }
        
        private void StartHeartbeat() 
        {
            if (_heartbeatCoroutine != null) return;

            SetCurrentRate(normalRate);
            _heartbeatCoroutine = StartCoroutine(HeartBeatCoroutine());
        }
        
        private void PlayFirstBeatSound()
        {
            GameManager.Instance.SoundSystem.PlayOneShot(beat1, _audioSources[0]);
        }
        
        private void PlaySecondBeatSound() 
        { 
            GameManager.Instance.SoundSystem.PlayOneShot(beat2, _audioSources[1]);
        }
        
        private float GetFirstBeatDelay()
        {
            return 60f / _currentRate - GetSecondBeatDelay();
        }
        
        private float GetSecondBeatDelay()
        {
            return Mathf.Clamp((60f / _currentRate) * 0.3f, 0.1f, 0.35f);
        }
        
        private void SetupAudioSources() 
        {
            _audioSources = new List<AudioSource>(GetComponentsInChildren<AudioSource>());
            GameManager.Instance.SoundSystem.SetupSound(_audioSources[0], beat1);
            GameManager.Instance.SoundSystem.SetupSound(_audioSources[1], beat2);
        }

        private void Awake()
        {
            SetupAudioSources();
        }
    }
}
