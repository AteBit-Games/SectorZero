/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections.Generic;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Runtime.InventorySystem
{
    public enum ActiveInventory
    {
        Items, 
        Tapes
    }
    
    public class InventoryManager : MonoBehaviour
    {
        [FormerlySerializedAs("isInventoryEnabled")]
        [Header("Items Inventory")]
        [SerializeField] public bool isInventoryScreenEnabled;
        [SerializeField] private PlayerInventory playerInventory;
        
        public PlayerInventory PlayerInventory => playerInventory;
        [HideInInspector] public bool isInventoryOpen;

        // Main Pause Items
        private UIDocument _uiDocument;
        private VisualElement _inventoryWindow;
        
        private Button _itemsButton;
        private Button _tapesButton;
        
        //Items Inventory
        private VisualElement _itemsInventoryContainer;
        
        private VisualElement _itemsInventoryListContainer;
        private readonly List<InventoryUIItem> _itemsInventoryList = new();

        private VisualElement _itemsInventoryInformation;
        private VisualElement _itemsInventoryInformationImage;
        private Label _itemsInventoryInformationTitle;
        private Label _itemsInventoryInformationDescription;
        
        //Tapes Inventory
        private VisualElement _tapesInventoryContainer;
        
        private VisualElement _tapesInventoryListContainer;
        private readonly List<InventoryUITape> _tapesInventoryList = new();
        
        private VisualElement _tapesInventoryInformation;
        private VisualElement _tapesInventoryInformationImage;
        private Label _tapesInventoryInformationTitle;
        private Label _tapesInventoryInformationDescription;
        private Button _tapesInventoryPlayButton;
        
        private ActiveInventory _activeInventory = ActiveInventory.Tapes;
        private Tape _activeTape;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            
            _inventoryWindow = rootVisualElement.Q<VisualElement>("inventory-window");

            //Buttons
            _itemsButton = _inventoryWindow.Q<Button>("items-toggle");
            _itemsButton.RegisterCallback<ClickEvent>(_ => SwitchToItemsInventory());
    
            _tapesButton = _inventoryWindow.Q<Button>("tapes-toggle");
            _tapesButton.RegisterCallback<ClickEvent>(_ => SwitchToTapesInventory());

            //Items
            _itemsInventoryContainer = _inventoryWindow.Q<VisualElement>("items-inventory");
            _itemsInventoryListContainer = _itemsInventoryContainer.Q<VisualElement>("item-list");

            _itemsInventoryInformation = _itemsInventoryContainer.Q<VisualElement>("item-information");
            _itemsInventoryInformationTitle = _itemsInventoryInformation.Q<Label>("item-title");
            _itemsInventoryInformationImage = _itemsInventoryInformation.Q<VisualElement>("item-image");
            _itemsInventoryInformationDescription = _itemsInventoryInformation.Q<Label>("item-description");

            //Tapes
            _tapesInventoryContainer = _inventoryWindow.Q<VisualElement>("tapes-inventory");
            _tapesInventoryListContainer = _tapesInventoryContainer.Q<VisualElement>("tape-list");
            
            _tapesInventoryInformation = _tapesInventoryContainer.Q<VisualElement>("tape-information");
            _tapesInventoryInformationTitle = _tapesInventoryInformation.Q<Label>("tape-title");
            _tapesInventoryInformationImage = _tapesInventoryInformation.Q<VisualElement>("tape-image");
            _tapesInventoryInformationDescription = _tapesInventoryInformation.Q<Label>("tape-description");
            _tapesInventoryPlayButton = _tapesInventoryInformation.Q<Button>("tape-listen");
            _tapesInventoryPlayButton.RegisterCallback<ClickEvent>(_ => ListenToTape(_activeTape));
        }

        public void OpenInventory()
        {
            Time.timeScale = 0;
            UIUtils.ShowUIElement(_inventoryWindow);
            isInventoryOpen = true;
            
            RegisterInventoryTapes();
            RegisterInventoryItems();
            SwitchToItemsInventory();
        }
        
        public void CloseInventory()
        {
            Time.timeScale = 1;
            isInventoryOpen = false;
            GameManager.Instance.ResetInput();
            GameManager.Instance.SoundSystem.ResumeAll();
            
            UIUtils.HideUIElement(_inventoryWindow);
        }

        private void SwitchToItemsInventory()
        {
            if(_activeInventory == ActiveInventory.Items) return;
            GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
            
            SelectItem(_itemsInventoryList[0].OnClick());
            _activeInventory = ActiveInventory.Items;
            UIUtils.ShowUIElement(_itemsInventoryContainer);
            _itemsInventoryContainer.pickingMode = PickingMode.Ignore;
            UIUtils.HideUIElement(_tapesInventoryContainer);
            _tapesInventoryContainer.pickingMode = PickingMode.Ignore;
            
            _itemsButton.BringToFront();
            _itemsButton.AddToClassList("inventory-toggle-active");
            _tapesButton.SendToBack();
            _tapesButton.RemoveFromClassList("inventory-toggle-active");
        }
        
        private void SwitchToTapesInventory()
        {
            if(_activeInventory == ActiveInventory.Tapes) return;
            GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());

            SelectTape(_tapesInventoryList[0].OnClick());
            _activeInventory = ActiveInventory.Tapes;
            UIUtils.HideUIElement(_itemsInventoryContainer);
            _itemsInventoryContainer.pickingMode = PickingMode.Ignore;
            UIUtils.ShowUIElement(_tapesInventoryContainer);
            _tapesInventoryContainer.pickingMode = PickingMode.Ignore;
            
            _tapesButton.BringToFront();
            _tapesButton.AddToClassList("inventory-toggle-active");
            _itemsButton.SendToBack();
            _itemsButton.RemoveFromClassList("inventory-toggle-active");
        }

        private void SelectItem(BaseItem item)
        {
            if (item == null)
            {
                _itemsInventoryInformationImage.style.backgroundImage = null;
                _itemsInventoryInformationTitle.text = "No Item Selected";
                _itemsInventoryInformationDescription.text = "";
            }
            else
            {
                _itemsInventoryInformationImage.style.backgroundImage = item.itemSprite.texture;
                _itemsInventoryInformationTitle.text = item.itemName;
                _itemsInventoryInformationDescription.text = item.itemDescription;
            }
        }
        
        private void SelectTape(BaseItem tape)
        {
            if (tape == null)
            {
                _tapesInventoryInformationImage.style.backgroundImage = null;
                _tapesInventoryInformationTitle.text = "No Item Selected";
                _tapesInventoryInformationDescription.text = "";
                _tapesInventoryPlayButton.style.display = DisplayStyle.None;
            }
            else
            {
                _tapesInventoryInformationImage.style.backgroundImage = tape.itemSprite.texture;
                _tapesInventoryInformationTitle.text = tape.itemName;
                _tapesInventoryInformationDescription.text = tape.itemDescription;
                _tapesInventoryPlayButton.style.display = DisplayStyle.Flex;
                
                _activeTape = (Tape)tape;
            }
        }
        
        private void RegisterInventoryItems()
        {
            var itemsList = _itemsInventoryListContainer.Query<VisualElement>("inventory-item").ToList();
            var index = 0;
            foreach (var item in itemsList)
            {
                _itemsInventoryList.Add(index < playerInventory.itemInventory.Count
                ? new InventoryUIItem(playerInventory.itemInventory[index], item)
                : new InventoryUIItem(null, item));
                
                var currentIndex = index;
                if(index < playerInventory.itemInventory.Count) item.RegisterCallback<ClickEvent>(_ => SelectItem(_itemsInventoryList[currentIndex].OnClick()));

                index++;
            }
        }

        private void RegisterInventoryTapes()
        {
            var tapesList = _tapesInventoryListContainer.Query<VisualElement>("inventory-tape").ToList();
            var index = 0;
            foreach (var tape in tapesList)
            {
                _tapesInventoryList.Add(index < playerInventory.tapeInventory.Count
                ? new InventoryUITape(playerInventory.tapeInventory[index], tape, playerInventory.tapeInventory[index].dialogue)
                : new InventoryUITape(null, tape, null));
            
                var currentIndex = index;
                if(index < playerInventory.tapeInventory.Count) tape.RegisterCallback<ClickEvent>(_ => SelectTape(_tapesInventoryList[currentIndex].OnClick()));
                
                index++;
            }
        }
        
        private void ListenToTape(Tape tape)
        {
            if(tape == null) return;
            CloseInventory();
            GameManager.Instance.DialogueSystem.StartDialogue(tape.dialogue);
        }
    }
}

