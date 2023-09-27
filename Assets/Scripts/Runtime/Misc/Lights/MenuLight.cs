/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Runtime.Misc
{
    [DefaultExecutionOrder(5)]
    public class MenuLight : MonoBehaviour
    {
        [SerializeField] private new Light2D light;

        [SerializeField] private float minOn = 3f;
        [SerializeField] private float maxOn = 6f;
        [SerializeField] private float minOff = 0.1f;
        [SerializeField] private float maxOff = 0.3f;
        
        private Coroutine _coroutine;
        
        //============================== Interface ==============================
        
        public void Start()
        {
            light.enabled = true;
            _coroutine = StartCoroutine(FlickerOff());
        }
        
        //============================== Coroutines ==============================
        
        private IEnumerator FlickerOff()
        {
            light.enabled = true;

            yield return new WaitForSecondsRealtime(Random.Range(minOn, maxOn));
            light.enabled = false;

            yield return new WaitForSecondsRealtime(Random.Range(minOff, maxOff));
            _coroutine = StartCoroutine(FlickerOff());
        }
    }
}
