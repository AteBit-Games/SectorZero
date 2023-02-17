using UnityEngine;

namespace Runtime.InventorySystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New_Weapon", menuName = "Inventory System/Items/Weapon")]
    public class Weapon : Item
    {
        [Space(10)]
        [Header("WEAPON DETAILS")]
        public RuntimeAnimatorController weaponAnimator; 
        public int damage;
        public bool isRanged;
    }
}

