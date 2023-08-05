/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using ElRaccoone.Tweens;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Runtime.Misc.Triggers
{
    public class PostProcessTrigger : MonoBehaviour
    {
        public void TriggerVignette(float delay)
        {
            var volume = FindObjectOfType<Volume>();
            var vignette = volume.sharedProfile.components[0] as Vignette;

            volume.TweenValueFloat(0.25f, 2f, value =>
            {
                if (vignette != null) vignette.intensity.value = value;
            }).SetFrom(0f).SetEaseSineInOut().SetDelay(delay);
        }
    }
}
