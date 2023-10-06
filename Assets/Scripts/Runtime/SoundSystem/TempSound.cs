/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using UnityEngine;
using UnityEngine.Audio;

namespace Runtime.SoundSystem
{
    [DefaultExecutionOrder(4)]
	public class TempSound : MonoBehaviour
	{
        [SerializeField] public AudioMixer mainMixer;
    }
}
