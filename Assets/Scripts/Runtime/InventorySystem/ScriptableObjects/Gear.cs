using UnityEngine;

namespace Runtime.InventorySystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New_GearItem", menuName = "Inventory System/Items/Gear Item")]
    public class Gear : Item
    {
        [Space(10)]
        [Header("GEAR DETAILS")]
        public int armorValue;
    }
}