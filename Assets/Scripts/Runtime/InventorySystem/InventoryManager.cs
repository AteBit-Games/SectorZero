/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.DialogueSystem;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        [Header("Item Info")] 
        [SerializeField] private GameObject itemName;
        [SerializeField] private GameObject itemViewImage;
        [SerializeField] private GameObject itemViewText;
        [SerializeField] private GameObject listenButton;

        [Header("Other")]
        [SerializeField] private GameObject inventoryItemPrefab;
        [SerializeField] private PlayerInventory playerInventory;
        
        private GameObject _selectedItem;
        
        public PlayerInventory PlayerInventory => playerInventory;
        
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
                    if (playerInventory.itemInventory.Count > 0) SelectItem(playerInventory.itemInventory[0]);
                    break;
                case ActiveInventory.Tapes:
                    itemsInventoryContainer.SetActive(false);
                    tapesInventoryContainer.SetActive(true);
                    if (playerInventory.tapeInventory.Count > 0) SelectItem(playerInventory.tapeInventory[0]);
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }

        private void MapFromPlayerInventoryToUI()
        {
            //Mapping tapes
            var tapeCount = playerInventory.tapeInventory.Count;
            foreach (Transform child in tapesInventoryList.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (var tape in playerInventory.tapeInventory)
            {
                var inventoryTape = Instantiate(inventoryItemPrefab, tapesInventoryList.transform);
                inventoryTape.GetComponent<InventoryUISlot>().InitializeItem(tape);
                inventoryTape.GetComponent<Button>().onClick.AddListener(delegate() { SelectItem(tape); });
            }
            for (var i = tapeCount; i < 16; i++) Instantiate(inventoryItemPrefab, tapesInventoryList.transform);

            //Mapping items
            var itemCount = playerInventory.itemInventory.Count;
            foreach (Transform child in itemsInventoryList.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (var item in playerInventory.itemInventory)
            {
                var inventoryItem = Instantiate(inventoryItemPrefab, itemsInventoryList.transform);
                inventoryItem.GetComponent<InventoryUISlot>().InitializeItem(item);
                inventoryItem.GetComponent<Button>().onClick.AddListener(delegate() { SelectItem(item); });
            }
            for (var i = itemCount; i < 16; i++) Instantiate(inventoryItemPrefab, itemsInventoryList.transform);

            // call the button click on the first item
            if (playerInventory.itemInventory.Count > 0) SelectItem(playerInventory.itemInventory[0]);
            else if (playerInventory.tapeInventory.Count > 0) SelectItem(playerInventory.tapeInventory[0]);
        }
        
        public void SelectItem(BaseItem item)
        {
            itemViewImage.SetActive(true);
            itemViewText.SetActive(true);
            itemViewImage.GetComponent<Image>().sprite = item.itemSprite;
            itemViewText.GetComponent<TMP_Text>().text = item.itemDescription;
            itemName.GetComponent<TMP_Text>().text = item.itemName;

            if (item.itemType == ItemType.TapeRecording)
            {
                listenButton.SetActive(true);
                listenButton.GetComponent<Button>().onClick.RemoveAllListeners();
                listenButton.GetComponent<Button>().onClick.AddListener(delegate() { ListenToTape((Tape) item); });
            }
            else
            {
                listenButton.SetActive(false);
            }
        }
        
        public void ListenToTape(Tape tape)
        {
            CloseInventory();
            GameManager.Instance.DialogueSystem.StartDialogue(tape.dialogue);
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
            GameManager.Instance.ResetInput();
            gameObject.SetActive(false);
        }
    }
}

