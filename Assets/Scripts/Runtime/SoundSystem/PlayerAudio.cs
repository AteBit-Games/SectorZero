/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.AI;
using Runtime.Misc;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Runtime.SoundSystem
{
    [Serializable]
    internal class FootstepSoundSet
    {
        public string tag;
        public List<Sound> footstepSounds;
    }
    
    public class PlayerAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<FootstepSoundSet> footstepSounds;
        [SerializeField] private float joggingRange = 18f;
        [SerializeField] private float sneakingRange = 6f;
        [SerializeField] private bool debug;

        private NoiseEmitter _noiseEmitter;
        private Tilemap _tilemap;

        //========================= Unity Events =========================//
        
        private void Awake()
        {
            _noiseEmitter = audioSource.GetComponent<NoiseEmitter>();
            _tilemap = FindFirstObjectByType<Tilemap>(FindObjectsInactive.Include);
        }
        
        //========================= Public Methods =========================//

        public void PlayFootstepSound()
        {
            var sound = DetermineSound();
            if(sound == null)
            {
                Debug.LogWarning("No sound found for footstep sound");
                return;
            }
            
            audioSource.volume = sound.volumeScale;
            audioSource.PlayOneShot(sound.clip);
            _noiseEmitter.Radius = joggingRange;
            _noiseEmitter.EmitLocal();
        }

        public void PlaySneakSound()
        {
            var sound = DetermineSound();
            
            
            if (sound == null)
            {
                Debug.LogWarning("No sound found for sneak sound");
                return;
            }
            
            audioSource.volume = sound.volumeScale / 2f;
            audioSource.PlayOneShot(sound.clip);
            _noiseEmitter.Radius = sneakingRange;
            _noiseEmitter.EmitLocal();
        }
        
        //========================= Private Methods =========================//

        private Sound DetermineSound()
        {
            var tile = _tilemap.GetTile(_tilemap.WorldToCell(transform.position)) as SoundTile;
            if (tile != null)
            {
                foreach(var footstepsList in from footstepSound in footstepSounds where footstepSound.tag == tile.tag select footstepSound.footstepSounds)
                {
                    return footstepsList[Random.Range(0, footstepsList.Count)];
                }
            }
            
            return null;
        }
        
        //========================= Unity Gizmos =========================//

        private void OnDrawGizmos()
        {
            if (debug)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, joggingRange);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, sneakingRange);
            }
        }
    }
}
