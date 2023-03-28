using UnityEngine;

namespace Runtime.InventorySystem.ScriptableObjects
{
    public class BaseItem : ScriptableObject
    {
        [Header("ITEM DETAILS")]
        public string itemName;
        public Sprite itemSprite;
        public ItemType itemType;
    }
}
