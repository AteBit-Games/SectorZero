using System.Collections.Generic;
using Runtime.InventorySystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.InventorySystem
{
    public class PlayerInventory : MonoBehaviour
    {
        // ============ Inventory Mapping ================
        private readonly List<Item> _playerInventory = new();
        
        #region HEADER
        [Space(10)]
        [Header("INVENTORY DETAILS")]
        [Space(3)]
        #endregion
        [SerializeField] 
        private int inventorySize = 3;

        private bool AddItemToInventory(Item item)
        {
            if (_playerInventory.Count < inventorySize)
            {
                _playerInventory.Add(item);
                return true;
            }
            else
            {
                return false;
            }
        }
        
        private void RemoveItemFromInventory(Item item)
        {
            if (_playerInventory.Contains(item))
            {
                _playerInventory.Remove(item);
            }
        }
        
        
        private void SetMaxInventorySize(int size)
        {
            if (size > inventorySize)
            {
                inventorySize = size;
            }
        }
    }
}

