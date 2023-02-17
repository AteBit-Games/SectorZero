using UnityEngine;

namespace Runtime.InventorySystem.ScriptableObjects
{
    public class Item : ScriptableObject
    {
        [Header("ITEM DETAILS")]
        public string itemName;
        public Sprite itemSprite;
        public int itemValue;
        public GearSlot gearSlot;
        
        [Space(10)]
        [TextArea(10, 10)]
        public string itemDescription;
    }
}
