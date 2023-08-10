/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Runtime.Utils
{
    public static class SoundUtils 
    {
        public static IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
        {
            float currentTime = 0;

            audioMixer.GetFloat(exposedParam, out var currentVol);
            currentVol = Mathf.Pow(10, currentVol / 20);
            var targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);
            
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                var newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
                audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
                yield return null;
            }
        }
    }
}