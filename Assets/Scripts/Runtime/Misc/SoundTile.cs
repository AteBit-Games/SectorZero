using UnityEngine;
using UnityEngine.Tilemaps;

namespace Runtime.Misc
{
    [CreateAssetMenu(fileName = "New SoundTile", menuName = "Tiles/SoundTile")]
    public class SoundTile : Tile
    {
        public string tag = "Concrete";
    }
}