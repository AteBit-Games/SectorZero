/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using Runtime.InteractionSystem.Objects;
using Runtime.InteractionSystem.Objects.UI;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.UI
{
    public enum UIType
    {
        Map,
        KeyPad,
        Note
    }
    
    [DefaultExecutionOrder(5)]
    public class HUD : Window
    {
        private UIDocument _uiDocument;
        private UIType _activeUIType;
        
        //Map
        private VisualElement _mapContainer;
        private VisualElement _map1;
        private VisualElement _map2;
        private VisualElement _map3;
        
        //Notes
        private VisualElement _notesContainer;
        
        //Research Note
        private VisualElement _researchNote;
        private Label _researchNoteContent;
        private Label _researchNoteTitle;
        private Label _researchNoteAuthor;
        private Label _researchNoteDate;
        
        //Hand Written Note
        private VisualElement _handWrittenNote;
        private Label _handWrittenNoteContent;
        private Label _handWrittenNoteAuthor;

        private VisualElement _monoView;
        private VisualElement _moonView;
        
        //KeyPad
        private Keypad _keyPad;
        private VisualElement _keyPadContainer;

        [HideInInspector] public bool isMapOpen;

        //=============================== Unity Events ===============================//
        
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            
            _mapContainer = rootVisualElement.Q<VisualElement>("map-view");
            _map1 = _mapContainer.Q<VisualElement>("map1");
            _map2 = _mapContainer.Q<VisualElement>("map2");
            _map3 = _mapContainer.Q<VisualElement>("map3");
             
            _notesContainer = rootVisualElement.Q<VisualElement>("note-view");
            SetupNotes(_notesContainer);
            
            _keyPad = GetComponent<Keypad>();
            _keyPadContainer = rootVisualElement.Q<VisualElement>("keypad-view");
            _keyPad.SetupKeypad(_keyPadContainer);
        }

        //=============================== Public Functions ===============================//

        public void OpenNote(Note note, bool standalone = false)
        {
            UIUtils.ShowUIElement(_notesContainer);

            switch (note.noteType)
            {
                case NoteType.Research:
                {
                    UIUtils.ShowUIElement(_researchNote);
                    UIUtils.HideUIElement(_handWrittenNote);
                    UIUtils.HideUIElement(_monoView);
                    UIUtils.HideUIElement(_moonView);
                
                    _researchNoteContent.text = note.content;
                    _researchNoteTitle.text = note.title;
                    _researchNoteAuthor.text = note.author;
                    _researchNoteDate.text = note.date;
                    break;
                }
                case NoteType.Handwritten:
                {
                    UIUtils.ShowUIElement(_handWrittenNote);
                    UIUtils.HideUIElement(_researchNote);
                    UIUtils.HideUIElement(_monoView);
                    UIUtils.HideUIElement(_moonView);
                
                    _handWrittenNoteContent.text = note.content;
                    _handWrittenNoteAuthor.text = note.author;
                    break;
                }
                case NoteType.MonolithNews:
                {
                    UIUtils.ShowUIElement(_monoView);
                    UIUtils.HideUIElement(_handWrittenNote);
                    UIUtils.HideUIElement(_researchNote);
                    UIUtils.HideUIElement(_moonView);
                    break;
                }
                case NoteType.MoonNews:
                {
                    UIUtils.ShowUIElement(_moonView);
                    UIUtils.HideUIElement(_handWrittenNote);
                    UIUtils.HideUIElement(_researchNote);
                    UIUtils.HideUIElement(_monoView);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (standalone)
            {
                GameManager.Instance.activeWindow = this;
                Time.timeScale = 0;
                GameManager.Instance.SoundSystem.PauseAll();
                GameManager.Instance.DisableInput();
                _activeUIType = UIType.Note;
            }
        }
        
        public void CloseNote()
        {
            UIUtils.HideUIElement(_notesContainer);
        }

        public void OpenMap(MapType mapType)
        {
            UIUtils.ShowUIElement(_mapContainer);
            
            switch (mapType)
            {
                case MapType.Type1:
                    UIUtils.ShowUIElement(_map1);
                    UIUtils.HideUIElement(_map2);
                    UIUtils.HideUIElement(_map3);
                    break;
                case MapType.Type2:
                    UIUtils.HideUIElement(_map1);
                    UIUtils.ShowUIElement(_map2);
                    UIUtils.HideUIElement(_map3);
                    break;
                case MapType.Type3:
                    UIUtils.HideUIElement(_map1);
                    UIUtils.HideUIElement(_map2);
                    UIUtils.ShowUIElement(_map3);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mapType), mapType, null);
            }
            
            GameManager.Instance.activeWindow = this;
            
            Time.timeScale = 0;
            GameManager.Instance.DisableInput();
            _activeUIType = UIType.Map;
            isMapOpen = true;
        }
        
        private void CloseMap()
        {
            isMapOpen = false;
            UIUtils.HideUIElement(_mapContainer);
        }
        
        public void OpenKeyPad(Safe safe)
        {
            UIUtils.ShowUIElement(_keyPadContainer);
            GameManager.Instance.activeWindow = this;
            _keyPad.OpenKeypad(safe);

            Time.timeScale = 0;
            GameManager.Instance.DisableInput();
            _activeUIType = UIType.KeyPad;
        }
        
        private void CloseKeyPad()
        {
            UIUtils.HideUIElement(_keyPadContainer);
        }
        
        //=============================== Helper Function ===============================//
        
        private void SetupNotes(VisualElement container)
        {
            _researchNote = container.Q<VisualElement>("research-note");
            _researchNoteContent = _researchNote.Q<Label>("note-content");
            _researchNoteTitle = _researchNote.Q<Label>("note-title");
            _researchNoteAuthor = _researchNote.Q<Label>("note-author");
            _researchNoteDate = _researchNote.Q<Label>("note-date");
            
            _handWrittenNote = container.Q<VisualElement>("handwritten-note");
            _handWrittenNoteContent = _handWrittenNote.Q<Label>("note-content");
            _handWrittenNoteAuthor = _handWrittenNote.Q<Label>("note-author");
            
            _moonView = _notesContainer.Q<VisualElement>("moon-news");
            _monoView = _notesContainer.Q<VisualElement>("monolith-news");
        }
        
        
        //=============================== Window Overrides ===============================//
        
        public override void OpenWindow()
        {
            throw new NotImplementedException();
        }

        public override void CloseWindow()
        {
            switch(_activeUIType)
            {
                case UIType.Map:
                    CloseMap();
                    break;
                case UIType.KeyPad:
                    CloseKeyPad();
                    break;
                case UIType.Note:
                    CloseNote();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Time.timeScale = 1;
            GameManager.Instance.ResetInput();
            GameManager.Instance.SoundSystem.ResumeAll();
            GameManager.Instance.activeWindow = null;
        }

        public override void CloseSubWindow()
        {
            throw new NotImplementedException();
        }
    }
}
