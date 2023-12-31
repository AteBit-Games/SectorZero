/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Managers;
using Runtime.Misc;
using Runtime.Misc.Triggers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Runtime.Player
{
    [Serializable]
    internal class FootstepSoundSet
    {
        public string tag;
        public List<Sound> footstepSounds;
    }
    
    [DefaultExecutionOrder(6)]
    public class PlayerAudio : MonoBehaviour, IPersistant
    {
        public AmbienceWing[] ambienceWings;
        public Wing activeWing;
        
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private List<FootstepSoundSet> footstepSounds;
        [SerializeField] private float joggingRange = 18f;
        [SerializeField] private bool debug;

        private NoiseEmitter _noiseEmitter;

        //========================= Unity Events =========================//
        
        private void Awake()
        {
            _noiseEmitter = audioSource.GetComponent<NoiseEmitter>();
            GameManager.Instance.SoundSystem.SetupSound(audioSource, footstepSounds[0].footstepSounds[0]);
        }

        private void Start()
        {
            if (SceneManager.GetActiveScene().name == "SectorTwo")
            {
                foreach (var wing in ambienceWings.Where(wing => wing.wing == activeWing))
                {
                    GameManager.Instance.SoundSystem.FadeToNextAmbience(wing.ambience);
                    break;
                }
            }
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
            GameManager.Instance.SoundSystem.PlayOneShot(sound, audioSource);
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
            GameManager.Instance.SoundSystem.PlayOneShot(sound, audioSource);
        }
        
        //========================= Private Methods =========================//

        private Sound DetermineSound()
        {
            var tile = tilemap.GetTile(tilemap.WorldToCell(transform.position)) as SoundTile;
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
            }
        }


        public string LoadData(SaveGame game)
        {
            activeWing = (Wing) Enum.Parse(typeof(Wing), game.playerData.activeWing);
            return "PlayerAudio";
        }

        public void SaveData(SaveGame game)
        {
            game.playerData.activeWing = activeWing.ToString();
        }
    }
}
