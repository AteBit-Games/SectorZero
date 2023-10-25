using System;
using System.Collections.Generic;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.UI
{
    [Serializable]
    public class InventoryItems
    {
        public List<Tape> tapeInventory = new();
        public List<Item> itemInventory = new();
        public List<Note> noteInventory = new();
        public List<SummaryEntry> summaryEntries = new();
    }
    
    public class CheatMenu : MonoBehaviour
    {
        [SerializeField] private InventoryItems inventoryItems;
        [SerializeField] private BehaviourTree.BehaviourTree buddyTree;
        [SerializeField] private BehaviourTree.BehaviourTree idleTree;
        [SerializeField] private BehaviourTree.BehaviourTree defaultTree;
        
        
        private UIDocument _uiDocument;
        private VisualElement _cheatMenu;
        
        private Button _addAll;
        private Button _disableBreakers;
        private Button _disableTree;
        private Button _companion;
        private Button _enableTree;
        private Button _enableBreakers;
        
        private Button _closeButton;
        private Label _description;

        //================================ Unity Events =================================//
        
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            
            _cheatMenu = rootVisualElement.Q<VisualElement>("cheat-menu");
            _description = rootVisualElement.Q<Label>("cheat-description");
            
            _addAll = rootVisualElement.Q<Button>("add-all-items");
            _addAll.RegisterCallback<ClickEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                GameManager.Instance.InventorySystem.PlayerInventory.AddAllItems(inventoryItems);
                GameManager.Instance.NotificationManager.ShowCheatEnabledNotification();
            });
            _addAll.RegisterCallback<MouseEnterEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
                _description.text = "Adds all items to inventory";
            });
            
            _disableBreakers = rootVisualElement.Q<Button>("disable-breakers");
            _disableBreakers.RegisterCallback<ClickEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                GameManager.Instance.PowerManager.DisableAllBreakers();
                GameManager.Instance.NotificationManager.ShowCheatEnabledNotification();
            });
            _disableBreakers.RegisterCallback<MouseEnterEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
                _description.text = "Disables all power breakers allowing you to use the elevator";
            });
            
            _disableTree = rootVisualElement.Q<Button>("disable-tree");
            _disableTree.RegisterCallback<ClickEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                GameManager.Instance.AIManager.monster.SetNewTree(idleTree);
                GameManager.Instance.NotificationManager.ShowCheatEnabledNotification();
            });
            _disableTree.RegisterCallback<MouseEnterEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
                _description.text = "Disables the monster AI - letting them stand idle so you can explore";
            });
            
            _companion = rootVisualElement.Q<Button>("companion-mode");
            _companion.RegisterCallback<ClickEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                GameManager.Instance.AIManager.monster.SetNewTree(buddyTree);
                GameManager.Instance.NotificationManager.ShowCheatEnabledNotification();
            });
            _companion.RegisterCallback<MouseEnterEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
                _description.text = "Makes the monster friendly - he will follow you around like a puppy";
            });
            
            _enableTree = rootVisualElement.Q<Button>("enable-tree");
            _enableTree.RegisterCallback<ClickEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                GameManager.Instance.AIManager.monster.ReturnToDefault(defaultTree);
                GameManager.Instance.NotificationManager.ShowCheatEnabledNotification();
            });
            _enableTree.RegisterCallback<MouseEnterEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
                _description.text = "Enables the monster AI - returning game to normal state";
            });
            
            _enableBreakers = rootVisualElement.Q<Button>("enable-breakers");
            _enableBreakers.RegisterCallback<ClickEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                GameManager.Instance.PowerManager.EnableAllBreakers();
                GameManager.Instance.NotificationManager.ShowCheatEnabledNotification();
            });
            _enableBreakers.RegisterCallback<MouseEnterEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
                _description.text = "Enables all power breakers";
            });
            
            _closeButton = rootVisualElement.Q<Button>("exit-button");
            _closeButton.RegisterCallback<ClickEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                HideMenu();
            });
            _closeButton.RegisterCallback<MouseEnterEvent>(_ => {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
        }
        
        // //================================ Public Methods =================================//

        public void ShowMenu()
        {
           UIUtils.ShowUIElement(_cheatMenu);
        }
        
        public void HideMenu()
        {
            UIUtils.HideUIElement(_cheatMenu);
        }
    }
}
