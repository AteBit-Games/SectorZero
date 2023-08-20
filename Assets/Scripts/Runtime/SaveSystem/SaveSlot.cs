/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using Runtime.Managers;
using Runtime.SaveSystem.Data;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.SaveSystem
{
    public class SaveSlot
    {
        private readonly VisualElement _activeSaveContainer;
        private readonly VisualElement _emptySaveContainer;
        
        protected SaveSlot(VisualElement saveSlotRef)
        {
            _activeSaveContainer = saveSlotRef.Q<VisualElement>("active-save");
            _emptySaveContainer = saveSlotRef.Q<VisualElement>("empty-save");
        }

        protected void SetInactive()
        {
            UIUtils.ShowUIElement(_emptySaveContainer);
            UIUtils.HideUIElement(_activeSaveContainer);
        }

        protected void SetActive()
        {
            UIUtils.ShowUIElement(_activeSaveContainer);
            UIUtils.HideUIElement(_emptySaveContainer);
        }
    }

    public class EmptySaveSlot : SaveSlot
    {
        public EmptySaveSlot(VisualElement saveSlotRef) : base(saveSlotRef)
        {
            SetInactive();
        }
    }
    
    public class ActiveSaveSlot : SaveSlot
    {
        public ActiveSaveSlot(SaveGame saveGame, Texture2D saveImage, VisualElement saveSlotRef, bool isLatest, Action onDelete) : base(saveSlotRef)
        {
            SetActive();
            
            var saveImageContainer = saveSlotRef.Q<VisualElement>("save-image");
            saveImageContainer.style.backgroundImage = saveImage;
            
            var saveName = saveSlotRef.Q<Label>("save-title");
            saveName.text = saveGame.saveName.SplitCamelCase();
            
            var saveTime = saveSlotRef.Q<Label>("save-time");
            saveTime.text = new DateTime(saveGame.saveTime).ToString("dd/MM/yyyy HH:mm");
            
            var deleteButton = saveSlotRef.Q<VisualElement>("delete-save");
            deleteButton.RegisterCallback<ClickEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                GameManager.Instance.SaveSystem.DeleteSave(saveGame.saveTime);
                onDelete.Invoke();
            });
            deleteButton.RegisterCallback<MouseEnterEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });
            
            var loadButton = saveSlotRef.Q<Button>("load-save-button");
            loadButton.RegisterCallback<ClickEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                GameManager.Instance.SaveSystem.LoadGame(saveGame.saveTime);
                GameManager.Instance.isMainMenu = false;
            });
            loadButton.RegisterCallback<MouseEnterEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.HoverSound());
            });

            if(isLatest)
            {
                var latestSave = saveSlotRef.Q<VisualElement>("is-latest-save");
                UIUtils.ShowUIElement(latestSave);
            }
        }
    }
}