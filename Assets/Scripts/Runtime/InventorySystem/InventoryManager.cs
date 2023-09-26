/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections.Generic;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.UI;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Runtime.InventorySystem
{
    public enum ActiveInventory
    {
        Items, 
        Tapes,
        Notes,
        Summary
    }
    
    public class InventoryManager : Window
    {
        [FormerlySerializedAs("isInventoryEnabled")]
        [Header("Items Inventory")]
        [SerializeField] public VisualTreeAsset summaryEntry;
        [SerializeField] public bool isInventoryScreenEnabled;
        [SerializeField] private PlayerInventory playerInventory;
        
        public PlayerInventory PlayerInventory => playerInventory;
        [HideInInspector] public bool isInventoryOpen;
        [HideInInspector] public bool isNoteWindowOpen;

        // Main Pause Items
        private UIDocument _uiDocument;
        private VisualElement _inventoryWindow;
        
        private Button _itemsButton;
        private Button _tapesButton;
        private Button _notesButton;
        private Button _summaryButton;
        
        //Items Inventory
        private VisualElement _itemsInventoryContainer;
        
        private VisualElement _itemsInventoryListContainer;
        private readonly List<InventorySlot> _itemsInventoryList = new();
        
        private VisualElement _itemsInventoryInformationImage;
        private Label _itemsInventoryInformationTitle;
        private Label _itemsInventoryInformationDescription;
        
        //Tapes Inventory
        private VisualElement _tapesInventoryContainer;
        
        private VisualElement _tapesInventoryListContainer;
        private readonly List<InventorySlot> _tapesInventoryList = new();
        
        private VisualElement _tapesInventoryInformationImage;
        private Label _tapesInventoryInformationTitle;
        private Label _tapesInventoryInformationDescription;
        private Button _tapesInventoryPlayButton;
        
        //Notes Inventory
        private VisualElement _notesInventoryContainer;
        
        private VisualElement _notesInventoryListContainer;
        private readonly List<InventorySlot> _notesInventoryList = new();
        
        private VisualElement _notesInventoryInformationImage;
        private Label _notesInventoryInformationTitle;
        private Label _notesInventoryInformationDescription;
        private Button _notesInventoryReadButton;
        
        //Summary Section
        private VisualElement _summarySectionContainer;
        private VisualElement _summarySectionListContainer;
        
        //Tracking
        private ActiveInventory _activeInventory = ActiveInventory.Tapes;
        private Tape _activeTape;
        private Note _activeNote;
        
        private InventorySlot _activeItemSlot;

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
            
            _notesButton = _inventoryWindow.Q<Button>("notes-toggle");
            _notesButton.RegisterCallback<ClickEvent>(_ => SwitchToNotesInventory());
            
            _summaryButton = _inventoryWindow.Q<Button>("summary-toggle");
            _summaryButton.RegisterCallback<ClickEvent>(_ => SwitchToSummaries());
            
            //Items
            _itemsInventoryContainer = _inventoryWindow.Q<VisualElement>("items-inventory");
            SetupItemsReferences(_itemsInventoryContainer);

            //Tapes
            _tapesInventoryContainer = _inventoryWindow.Q<VisualElement>("tapes-inventory");
            SetupTapesReferences(_tapesInventoryContainer);
            
            //Notes
            _notesInventoryContainer = _inventoryWindow.Q<VisualElement>("notes-inventory");
            SetupNotesReferences(_notesInventoryContainer);
            
            //Summary
            _summarySectionContainer = _inventoryWindow.Q<VisualElement>("summaries-section");
            SetupSummaryReferences(_summarySectionContainer);
        }

        public override void OpenWindow()
        {
            GameManager.Instance.SoundSystem.PauseAll();
            GameManager.Instance.DisableInput();
            Time.timeScale = 0;
            UIUtils.ShowUIElement(_inventoryWindow);
            isInventoryOpen = true;
            
            RegisterInventoryTapes();
            RegisterInventoryItems();
            RegisterInventoryNotes();
            RegisterSummaries();
            
            _activeInventory = ActiveInventory.Tapes;
            SwitchToItemsInventory();
        }
        
        public override void CloseWindow()
        {
            if (isNoteWindowOpen) return;
            
            Time.timeScale = 1;
            isInventoryOpen = false;
            GameManager.Instance.activeWindow = null;
            
            UIUtils.HideUIElement(_inventoryWindow);
            GameManager.Instance.ResetInput();
            GameManager.Instance.SoundSystem.ResumeAll();
        }

        public override void CloseSubWindow()
        {
            GameManager.Instance.HUD.CloseNote();
            isNoteWindowOpen = false;
            isSubWindowOpen = false;
        }

        private void SwitchToItemsInventory()
        {
            if(_activeInventory == ActiveInventory.Items) return;
            GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
            
            _activeItemSlot?.Deselect();
            SelectItem(_itemsInventoryList[0].OnClick());
            _activeItemSlot = _itemsInventoryList[0];

            _activeInventory = ActiveInventory.Items;
            UIUtils.ShowUIElement(_itemsInventoryContainer);
            _itemsInventoryContainer.pickingMode = PickingMode.Ignore;
            UIUtils.HideUIElement(_tapesInventoryContainer);
            _tapesInventoryContainer.pickingMode = PickingMode.Ignore;
            UIUtils.HideUIElement(_notesInventoryContainer);
            _notesInventoryContainer.pickingMode = PickingMode.Ignore;
            UIUtils.HideUIElement(_summarySectionContainer);
            _summarySectionContainer.pickingMode = PickingMode.Ignore;
            
            _itemsButton.BringToFront();
            _itemsButton.AddToClassList("inventory-toggle-active");
            _tapesButton.SendToBack();
            _tapesButton.RemoveFromClassList("inventory-toggle-active");
            _notesButton.SendToBack();
            _notesButton.RemoveFromClassList("inventory-toggle-active");
            _summaryButton.SendToBack();
            _summaryButton.RemoveFromClassList("inventory-toggle-active");
        }
        
        private void SwitchToTapesInventory()
        {
            if(_activeInventory == ActiveInventory.Tapes) return;
            GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());

            _activeItemSlot?.Deselect();
            SelectTape(_tapesInventoryList[0].OnClick());
            _activeItemSlot = _tapesInventoryList[0];
            
            _activeInventory = ActiveInventory.Tapes;
            UIUtils.HideUIElement(_itemsInventoryContainer);
            _itemsInventoryContainer.pickingMode = PickingMode.Ignore;
            UIUtils.ShowUIElement(_tapesInventoryContainer);
            _tapesInventoryContainer.pickingMode = PickingMode.Ignore;
            UIUtils.HideUIElement(_notesInventoryContainer);
            _notesInventoryContainer.pickingMode = PickingMode.Ignore;
            UIUtils.HideUIElement(_summarySectionContainer);
            _summarySectionContainer.pickingMode = PickingMode.Ignore;
            
            _tapesButton.BringToFront();
            _tapesButton.AddToClassList("inventory-toggle-active");
            _itemsButton.SendToBack();
            _itemsButton.RemoveFromClassList("inventory-toggle-active");
            _notesButton.SendToBack();
            _notesButton.RemoveFromClassList("inventory-toggle-active");
            _summaryButton.SendToBack();
            _summaryButton.RemoveFromClassList("inventory-toggle-active");
        }
        
        private void SwitchToNotesInventory()
        {
            if(_activeInventory == ActiveInventory.Notes) return;
            GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
            
            _activeItemSlot?.Deselect();
            SelectNote(_notesInventoryList[0].OnClick());
            _activeItemSlot = _notesInventoryList[0];
            
            _activeInventory = ActiveInventory.Notes;
            UIUtils.HideUIElement(_itemsInventoryContainer);
            _itemsInventoryContainer.pickingMode = PickingMode.Ignore;
            UIUtils.HideUIElement(_tapesInventoryContainer);
            _tapesInventoryContainer.pickingMode = PickingMode.Ignore;
            UIUtils.ShowUIElement(_notesInventoryContainer);
            _notesInventoryContainer.pickingMode = PickingMode.Ignore;
            UIUtils.HideUIElement(_summarySectionContainer);
            _summarySectionContainer.pickingMode = PickingMode.Ignore;
            
            _notesButton.BringToFront();
            _notesButton.AddToClassList("inventory-toggle-active");
            _tapesButton.SendToBack();
            _tapesButton.RemoveFromClassList("inventory-toggle-active");
            _itemsButton.SendToBack();
            _itemsButton.RemoveFromClassList("inventory-toggle-active");
            _summaryButton.SendToBack();
            _summaryButton.RemoveFromClassList("inventory-toggle-active");
        }

        private void SwitchToSummaries()
        {
            if(_activeInventory == ActiveInventory.Summary) return;
            GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
            
            _activeInventory = ActiveInventory.Summary;
            UIUtils.HideUIElement(_itemsInventoryContainer);
            _itemsInventoryContainer.pickingMode = PickingMode.Ignore;
            UIUtils.HideUIElement(_tapesInventoryContainer);
            _tapesInventoryContainer.pickingMode = PickingMode.Ignore;
            UIUtils.HideUIElement(_notesInventoryContainer);
            _notesInventoryContainer.pickingMode = PickingMode.Ignore;
            UIUtils.ShowUIElement(_summarySectionContainer);
            _summarySectionContainer.pickingMode = PickingMode.Ignore;
            
            _summaryButton.BringToFront();
            _summaryButton.AddToClassList("inventory-toggle-active");
            _notesButton.SendToBack();
            _notesButton.RemoveFromClassList("inventory-toggle-active");
            _tapesButton.SendToBack();
            _tapesButton.RemoveFromClassList("inventory-toggle-active");
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
                _tapesInventoryInformationTitle.text = "No Tape Selected";
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
        
        private void SelectNote(BaseItem note)
        {
            if (note == null)
            {
                _notesInventoryInformationImage.style.backgroundImage = null;
                _notesInventoryInformationTitle.text = "No Note Selected";
                _notesInventoryInformationDescription.text = "";
                _notesInventoryReadButton.style.display = DisplayStyle.None;
            }
            else
            {
                _notesInventoryInformationImage.style.backgroundImage = note.itemSprite.texture;
                _notesInventoryInformationTitle.text = note.itemName;
                _notesInventoryInformationDescription.text = note.itemDescription;
                _notesInventoryReadButton.style.display = DisplayStyle.Flex;
                
                _activeNote = (Note)note;
            }
        }
        
        private void RegisterInventoryItems()
        {
            _itemsInventoryList.Clear();
            var itemsList = _itemsInventoryListContainer.Query<VisualElement>("inventory-item").ToList();
            var index = 0;
            foreach (var item in itemsList)
            {
                _itemsInventoryList.Add(index < playerInventory.itemInventory.Count
                ? new InventorySlot(playerInventory.itemInventory[index], item)
                : new InventorySlot(null, item));

                var currentIndex = index;
                if (index < playerInventory.itemInventory.Count)
                {
                    item.RegisterCallback<ClickEvent>(_ =>
                    {
                        if(_activeItemSlot != _itemsInventoryList[currentIndex])
                        {
                            _activeItemSlot?.Deselect();
                            _activeItemSlot = _itemsInventoryList[currentIndex];
                            SelectItem(_itemsInventoryList[currentIndex].OnClick());
                            GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                        }
                    });
                    item.RegisterCallback<MouseEnterEvent>(_ =>
                    {
                        if(_activeItemSlot != _itemsInventoryList[currentIndex]) GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
                    });
                }

                index++;
            }
        }

        private void RegisterInventoryTapes()
        {
            _tapesInventoryList.Clear();
            var tapesList = _tapesInventoryListContainer.Query<VisualElement>("inventory-tape").ToList();
            var index = 0;
            foreach (var tape in tapesList)
            {
                _tapesInventoryList.Add(index < playerInventory.tapeInventory.Count
                ? new InventorySlot(playerInventory.tapeInventory[index], tape)
                : new InventorySlot(null, tape));
            
                var currentIndex = index;
                if(index < playerInventory.tapeInventory.Count){
                    tape.RegisterCallback<ClickEvent>(_ =>
                    {
                        if(_activeItemSlot != _tapesInventoryList[currentIndex])
                        {
                            _activeItemSlot?.Deselect();
                            _activeItemSlot = _tapesInventoryList[currentIndex];
                            SelectTape(_tapesInventoryList[currentIndex].OnClick());
                            GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                        }
                    });
                    tape.RegisterCallback<MouseEnterEvent>(_ => GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound()));
                }

                index++;
            }
        }
        
        private void RegisterInventoryNotes()
        {
            _notesInventoryList.Clear();
            var notesList = _notesInventoryListContainer.Query<VisualElement>("inventory-note").ToList();
            var index = 0;
            foreach (var note in notesList)
            {
                _notesInventoryList.Add(index < playerInventory.noteInventory.Count
                    ? new InventorySlot(playerInventory.noteInventory[index], note)
                    : new InventorySlot(null, note));
            
                var currentIndex = index;
                if(index < playerInventory.noteInventory.Count){
                    note.RegisterCallback<ClickEvent>(_ =>
                    {
                        if(_activeItemSlot != _notesInventoryList[currentIndex])
                        {
                            _activeItemSlot?.Deselect();
                            _activeItemSlot = _notesInventoryList[currentIndex];
                            SelectNote(_notesInventoryList[currentIndex].OnClick());
                            GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                        }
                    });
                    note.RegisterCallback<MouseEnterEvent>(_ => GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound()));
                }

                index++;
            }
        }

        private void RegisterSummaries()
        {
            if (playerInventory.summaryEntries.Count == 0)
            {
                var label = new Label("No Summaries Found");
                label.AddToClassList("no-summaries");
                _summarySectionListContainer.Add(label);
                return;
            }
            
            _summarySectionListContainer.Clear();
            foreach (var summary in playerInventory.summaryEntries)
            {
                var summaryInstance = summaryEntry.CloneTree();
                
                //Set content
                var summaryText = summaryInstance.Q<Label>("summary-text");
                summaryText.text = summary.summaryContent;

                //Set completed
                var summaryComplete = summaryInstance.Q<VisualElement>("summary-line");
                summaryComplete.style.visibility = summary.isCompleted ? Visibility.Visible : Visibility.Hidden;
                
                //add to list
                _summarySectionListContainer.Add(summaryInstance);
            }
        }
        
        //========================================== Helper Methods ==========================================//
        
        private void SetupNotesReferences(VisualElement notesInventoryContainer)
        {
            _notesInventoryListContainer = notesInventoryContainer.Q<VisualElement>("note-list");
            _notesInventoryInformationTitle = notesInventoryContainer.Q<Label>("note-title");
            _notesInventoryInformationImage = notesInventoryContainer.Q<VisualElement>("note-image");
            _notesInventoryInformationDescription = notesInventoryContainer.Q<Label>("note-description");
            
            //Button to read note
            _notesInventoryReadButton = notesInventoryContainer.Q<Button>("note-read");
            _notesInventoryReadButton.RegisterCallback<ClickEvent>(_ => ReadNote());
        }

        private void SetupTapesReferences(VisualElement tapesInventoryContainer)
        {
            _tapesInventoryListContainer = tapesInventoryContainer.Q<VisualElement>("tape-list");
            _tapesInventoryInformationTitle = tapesInventoryContainer.Q<Label>("tape-title");
            _tapesInventoryInformationImage = tapesInventoryContainer.Q<VisualElement>("tape-image");
            _tapesInventoryInformationDescription = tapesInventoryContainer.Q<Label>("tape-description");
            
            //Button to play tape
            _tapesInventoryPlayButton = tapesInventoryContainer.Q<Button>("tape-listen");
            _tapesInventoryPlayButton.RegisterCallback<ClickEvent>(_ => ListenToTape(_activeTape));
        }

        private void SetupItemsReferences(VisualElement itemsInventoryContainer)
        {
            _itemsInventoryListContainer = itemsInventoryContainer.Q<VisualElement>("item-list");

            _itemsInventoryInformationTitle = itemsInventoryContainer.Q<Label>("item-title");
            _itemsInventoryInformationImage = itemsInventoryContainer.Q<VisualElement>("item-image");
            _itemsInventoryInformationDescription = itemsInventoryContainer.Q<Label>("item-description");
        }
        
        private void SetupSummaryReferences(VisualElement summarySectionContainer)
        {
            _summarySectionListContainer = summarySectionContainer.Q<VisualElement>("summary-list");
        }
        
        //========================================== Interaction Events ==========================================//
        
        private void ListenToTape(Tape tape)
        {
            if(tape == null) return;
            CloseWindow();
            GameManager.Instance.DialogueSystem.StartDialogue(tape.dialogue);
        }
        
        private void ReadNote()
        {
            if(_activeNote == null) return;
            
            GameManager.Instance.HUD.OpenNote(_activeNote);
            isNoteWindowOpen = true;
            isSubWindowOpen = true;
        }
    }
}

