using System.Collections;
using System.Collections.Generic;
using Runtime.Managers;
using UnityEngine;

public class AmbienceTrigger : MonoBehaviour
{
    public AudioClip ambClip;
    
    
    public void TriggerAmbience(float fadeTime)
    {
        GameManager.Instance.AmbienceManager.FadeToNext(ambClip, fadeTime);
    } 
}
