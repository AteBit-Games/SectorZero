// Copyright (c) 2023 AteBit Games

using UnityEngine;
using System.Collections.Generic;
using Runtime.SoundSystem.ScriptableObjects;

namespace Runtime.SoundSystem
{
    public class PlayerAudio : MonoBehaviour 
    {
        private Dictionary<PhysicMaterial, SurfaceSound> _surfaceTable;
        
        [System.Serializable]
        public class SurfaceSound
        {
            [Tooltip("The material that this set applies to.")]
            public PhysicMaterial surfaceMaterial; 
            [Tooltip("Default footstep sound. Used if no material specific sound exists.")]
            public AudioCue footstepCue;
        }

        [Tooltip("Default sounds to play if there is no material specific sound.")]
        public SurfaceSound defaultSounds = new();
        [Tooltip("List of surface specific sounds.")]
        public List<SurfaceSound> surfaceSounds = new();
        
        private void OnEnable()
        {	
            int numSurfaces = surfaceSounds.Count;
            for(int surfaceIndex = 0; surfaceIndex < numSurfaces; ++surfaceIndex)
            {
                SurfaceSound surfaceSound = surfaceSounds[surfaceIndex];
                if(surfaceSound.surfaceMaterial != null)
                {
                    _surfaceTable ??= new Dictionary<PhysicMaterial, SurfaceSound>();
                    _surfaceTable[surfaceSound.surfaceMaterial] = surfaceSound;
                }
            }
        }

        private void OnDisable()
        {
            _surfaceTable = null;
        }

        private void OnFootstep(PhysicMaterial currentMaterial)
        {
            SurfaceSound surfaceSound = GetCurrentSurface(currentMaterial);
            AudioManager.Play(surfaceSound.footstepCue, transform.position, false);
        }
        
        private SurfaceSound GetCurrentSurface(PhysicMaterial currentMaterial)
        {
            if(currentMaterial != null && _surfaceTable != null && _surfaceTable.TryGetValue(currentMaterial, out SurfaceSound surfaceSound))
            {
                return surfaceSound;
            }
            else
            {
                return defaultSounds;
            }
        }
    }
}