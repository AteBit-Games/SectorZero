using System.Collections;
using System.Collections.Generic;
using Runtime.Managers;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;

public class AmbienceTrigger : MonoBehaviour
{
    public Sound ambClip;
    
    
    public void TriggerAmbience(float fadeTime)
    {
        GameManager.Instance.AmbienceManager.FadeToNext(ambClip, fadeTime);
    } 
    
    public void TriggerFadeOut(float fadeTime)
    {
        GameManager.Instance.AmbienceManager.FadeOut(fadeTime);
    }
    
    public void TriggerSilence()
    {
        GameManager.Instance.AmbienceManager.SilenceAmbience();
    }
}
