using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Runtime.Managers
{
    public class LightManager : MonoBehaviour
    {
        public SectorList lightSectors = new();
        
        public void DisableSector(int sectorIndex)
        {
            foreach (var light2D in lightSectors.list[sectorIndex].lights)
            {
                light2D.enabled = false;
            }
        }
        
        public void EnableSector(int sectorIndex)
        {
            foreach (var light2D in lightSectors.list[sectorIndex].lights)
            {
                light2D.enabled = true;
            }
        }
        
        public void DisableAllSectors()
        {
            foreach (var sector in lightSectors.list)
            {
                foreach (var light2D in sector.lights)
                {
                    light2D.enabled = false;
                }
            }
        }
    }
    
    [System.Serializable]
    public class Sector
    {
        public List<Light2D> lights;

        public Sector()
        {
            lights = new List<Light2D>();
        }
    }
 
    [System.Serializable]
    public class SectorList
    {
        public List<Sector> list;
    }

}
