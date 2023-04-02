// Copyright (c) 2023 AteBit Games
using UnityEngine;

namespace Runtime.SoundSystem
{
    public class TriggerSource : PointSource
    { 
        private GameObject _activator = null;
        
        public TriggerSource()
        {
            loop = false;
            playOnStart = false;
        }
        
        private void OnEnable()
        {
            if(!isPlaying && _activator)
            {
                Play();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(_activator == null)
            {
                Play();
                _activator = other.gameObject;
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if(_activator == other.gameObject)
            {
                Stop(false);
                _activator = null;
            }
        }
    }
}