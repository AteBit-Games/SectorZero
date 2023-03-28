using UnityEngine;

namespace Runtime.InventorySystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/Item")]
    public class Item : BaseItem
    {
        public string itemDescription;
    }
}