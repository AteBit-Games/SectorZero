using System;
using System.Linq;
using System.Collections.Generic;
using Runtime.InventorySystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.InventorySystem
{
    public class PlayerInventory : MonoBehaviour
    {
        // ============ Inventory Mapping ================
        private static readonly Item EmptyItem;
        private readonly Dictionary<GearSlot, Item> _playerGear = Enum.GetValues(typeof(GearSlot)).Cast<GearSlot>().ToDictionary(t => t, _ => EmptyItem);
        private readonly List<Item> _playerInventory = new();
        

        #region HEADER
        [Space(10)]
        [Header("INVENTORY DETAILS")]
        [Space(3)]
        #endregion
        [SerializeField] 
        private int gold = 0;
        [SerializeField] 
        private int gems = 0;
        [SerializeField] 
        private int scraps = 0;
        [SerializeField]
        private Animator weaponAnimator;
        [SerializeField] 
        private RuntimeAnimatorController defaultWeaponAnimator;
        [SerializeField] 
        private int inventorySize = 3;
        
        #region HEADER
        [Space(10)]
        [Header("STARTING GEAR")]
        [Space(3)]
        #endregion
        public Item[] startingGear;

        private void Start()
        {
            foreach (var item in startingGear)
            {
                EquipItem(item);
            }
        }

        private void EquipItem(Item item)
        {
            if (_playerGear[item.gearSlot] == EmptyItem)
            {
                _playerGear[item.gearSlot] = item;  
            }
            else
            {
                UnequipItem(item);
                _playerGear[item.gearSlot] = item;
            }
            
            if (item is Weapon weapon)
            {
                weaponAnimator.runtimeAnimatorController = weapon.weaponAnimator;
            }
        }
        
        private void EquipItemFromInventory(Item item)
        {
            if (_playerInventory.Contains(item))
            {
                RemoveItemFromInventory(item);
                EquipItem(item);
            }
            else
            {
                Debug.Log("Item not found in inventory");
            }
        }
        
        private void UnequipItem(Item item)
        {
            if (_playerGear[item.gearSlot] != EmptyItem)
            {
                if (AddItemToInventory(_playerGear[item.gearSlot]))
                {
                    _playerGear[item.gearSlot] = EmptyItem;
                }
                else
                {
                    // Inventory is full
                }
                
                // if(item.gearSlot == GearSlot.Weapon)
                // {
                //     weaponAnimator.runtimeAnimatorController = defaultWeaponAnimator;
                // }
            }
        }
        
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
        
        private void AddGold(int amount)
        {
            gold += amount;
        }
        
        private void RemoveGold(int amount)
        {
            gold -= amount;
        }
        
        private void AddGems(int amount)
        {
            gems += amount;
        }
        
        private void RemoveGems(int amount)
        {
            gems -= amount;
        }
        
        private void AddScraps(int amount)
        {
            scraps += amount;
        }
        
        private void RemoveScraps(int amount)
        {
            scraps -= amount;
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

