/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using UnityEngine;

namespace Runtime.InventorySystem
{
    public enum ActiveInventory
    {
        Items, 
        Tapes
    }
    
    public class InventoryManager : MonoBehaviour
    {
        [Header("Items Inventory")]
        [SerializeField] private GameObject itemsInventoryContainer;
        [SerializeField] private GameObject itemsInventoryList;
        
        [Header("Tapes Inventory")]
        [SerializeField] private GameObject tapesInventoryContainer;
        [SerializeField] private GameObject tapesInventoryList;
        
        [Header("Other")]
        [SerializeField] private GameObject inventoryItemPrefab;
        [SerializeField] private PlayerInventory playerInventory;
        private void Start()
        {
            itemsInventoryContainer.SetActive(true);
            tapesInventoryContainer.SetActive(false);
        }

        public void SwitchToItemsInventory()
        {
            SwitchInventory(ActiveInventory.Items);
        }
        
        public void SwitchToTapesInventory()
        {
            SwitchInventory(ActiveInventory.Tapes);
        }
        
        private void SwitchInventory(ActiveInventory activeInventory)
        {
            switch (activeInventory)
            {
                case ActiveInventory.Items:
                    itemsInventoryContainer.SetActive(true);
                    tapesInventoryContainer.SetActive(false);
                    break;
                case ActiveInventory.Tapes:
                    itemsInventoryContainer.SetActive(false);
                    tapesInventoryContainer.SetActive(true);
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }

        private void MapFromPlayerInventoryToUI()
        {
            //Mapping tapes
            var tapeCount = playerInventory.TapeInventory.Count;
            foreach (Transform child in tapesInventoryList.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (var tape in playerInventory.TapeInventory)
            {
                var tapeItem = Instantiate(inventoryItemPrefab, tapesInventoryList.transform);
                tapeItem.GetComponent<InventoryUISlot>().InitializeItem(tape);
            }
            for (var i = tapeCount; i < 16; i++) Instantiate(inventoryItemPrefab, tapesInventoryList.transform);

            //Mapping items
            var itemCount = playerInventory.ItemInventory.Count;
            foreach (Transform child in itemsInventoryList.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (var item in playerInventory.ItemInventory)
            {
                var tapeItem = Instantiate(inventoryItemPrefab, itemsInventoryList.transform);
                tapeItem.GetComponent<InventoryUISlot>().InitializeItem(item);
            }
            for (var i = itemCount; i < 16; i++) Instantiate(inventoryItemPrefab, itemsInventoryList.transform);
        }

        public void OpenInventory()
        {
            Time.timeScale = 0;
            gameObject.SetActive(true);
            MapFromPlayerInventoryToUI();
        }
        
        public void CloseInventory()
        {
            
            Time.timeScale = 1;
            gameObject.SetActive(false);
        }
    }
}

