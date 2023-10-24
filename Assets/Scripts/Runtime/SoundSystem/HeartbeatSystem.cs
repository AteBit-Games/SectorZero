/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Runtime.Managers;
using UnityEngine.Serialization;

namespace Runtime.SoundSystem
{
    [DefaultExecutionOrder(6)]
	public class HeartBeatSystem : MonoBehaviour
	{
        [SerializeField] private int normalRate = 60;
        [SerializeField] private int maxRate = 140;
        [SerializeField] private Sound beat1;
        [SerializeField] private Sound beat2;
        [SerializeField] private int changingSpeed = 3;
        [SerializeField] private AnimationCurve volumeCurve;
        
        //----- Private Variables -----//
        private List<AudioSource> _audioSources;
        private Coroutine _heartbeatCoroutine;
        private Coroutine _heartBeatIncreaseCoroutine;
        public int currentRate;
        private int _targetRate;
        
        public void Enable() 
        { 
            StartHeartbeat(); 
        }
        
        public void Disable()
        {
            StopIncrease();
            StopHeartbeat();
        }
        
        public void SetHeartRateSmoothly(int targetRate)
        {
            if (currentRate == targetRate) return;
            StopIncrease();
            SetTargetRate(targetRate);
            _heartBeatIncreaseCoroutine = StartCoroutine(HeartBeatIncreaseCoroutine());
        }

        /// <summary> Increase the heart rate by passed value smoothly </summary>
        public void ChangeHeartRateSmoothlyByValue(int rate)
        {
            if (_heartBeatIncreaseCoroutine == null)
                rate += currentRate;
            else
                rate += _targetRate;

            SetHeartRateSmoothly(rate);
        }
        
        private void SetCurrentRate(int rate) 
        {
            rate = Mathf.Clamp(rate, normalRate, maxRate);
            
            currentRate = rate;
            UpdateHeartBeatVolume();
        }
        
        private IEnumerator HeartBeatCoroutine() 
        {
            while (true) 
            {
                PlayFirstBeatSound();
                yield return new WaitForSeconds(GetSecondBeatDelay());
                PlaySecondBeatSound();
                yield return new WaitForSeconds(GetFirstBeatDelay());
            }
        }
        
        private IEnumerator HeartBeatIncreaseCoroutine()
        {
            while (Mathf.Abs(currentRate - _targetRate) > changingSpeed)
            {
                yield return new WaitForSecondsRealtime(1);
                SetCurrentRate(currentRate > _targetRate ? currentRate - changingSpeed : currentRate + changingSpeed);
            }

            if (currentRate != _targetRate)
            {
                yield return new WaitForSecondsRealtime(1);
                SetCurrentRate(_targetRate);
            }
            
            _heartBeatIncreaseCoroutine = null;
        }
        
        private void UpdateHeartBeatVolume()
        {
            var ratePercent = (currentRate - normalRate) / (float)(maxRate - normalRate);
            var volume = Mathf.Clamp(volumeCurve.Evaluate(ratePercent), 0, 1);

            foreach (var source in _audioSources) source.volume = volume;
        }
        
        private void StopIncrease() 
        {
            if (_heartBeatIncreaseCoroutine == null) return;
            StopCoroutine(_heartBeatIncreaseCoroutine);
            _heartBeatIncreaseCoroutine = null;
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
        
        private void SetTargetRate(int targetRate)
        {
            targetRate = Mathf.Clamp(targetRate, normalRate, maxRate);
            _targetRate = targetRate;
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
            return 60f / currentRate - GetSecondBeatDelay();
        }
        
        private float GetSecondBeatDelay()
        {
            return Mathf.Clamp((60f / currentRate) * 0.3f, 0.1f, 0.28f);
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