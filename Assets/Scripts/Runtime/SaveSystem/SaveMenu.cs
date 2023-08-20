/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections.Generic;
using Runtime.Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.SaveSystem
{
    [DefaultExecutionOrder(10)]
    public class SaveMenu : MonoBehaviour
    {
        public VisualTreeAsset saveInstance;
        
        private UIDocument _uiDocument;

        private VisualElement _saveList;
        private List<SaveSlot> _saveSlots = new();
        
        private Button _backButton;
        private Label _buttonDescription;
        
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement.Q<VisualElement>("saves-window");
            _buttonDescription = rootVisualElement.Q<Label>("button-description");
            _buttonDescription.text = "Exit the load save menu";
            
            _saveList = rootVisualElement.Q<VisualElement>("saves-list");
            
            _backButton = rootVisualElement.Q<Button>("back-button");
            _backButton.RegisterCallback<MouseEnterEvent>(_ =>
            {
                _buttonDescription.text = "Exit the load save menu";
            });
            _backButton.RegisterCallback<ClickEvent>(_ =>
            {
                GameManager.Instance.HandleEscape();
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
            });
        }

        public void ShowSaves()
        {
            _saveList.Clear();
            
            var index = 0;
            var saveData = GameManager.Instance.SaveSystem.GetSaveGames();
            foreach (var save in saveData)
            {
                var saveSlot = saveInstance.CloneTree();
                _saveSlots.Add(new ActiveSaveSlot(save.Value, save.Key, saveSlot, index == 0, ShowSaves));
                _saveList.Add(saveSlot);
                index++;
            }

            if(index < 3)
            {
                //get it up to 3 slots
                for(var i = index; i < 3; i++)
                {
                    var saveSlot = saveInstance.CloneTree();
                    _saveSlots.Add(new EmptySaveSlot(saveSlot));
                    _saveList.Add(saveSlot);
                }
            }
        }
    }
}