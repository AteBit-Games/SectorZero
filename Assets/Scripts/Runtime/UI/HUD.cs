/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using Runtime.InteractionSystem.Objects.UI;
using Runtime.InventorySystem.ScriptableObjects;
using Runtime.Managers;
using Runtime.SoundSystem;
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
    
    public class HUD : Window
    {
        [SerializeField] private Sound keyPressSound;
        [SerializeField] private Sound keyPressErrorSound;
        [SerializeField] private Sound keyPadSuccessSound;
        [SerializeField] private Sound keyPadErrorSound;
        
        
        private UIDocument _uiDocument;
        private UIType _activeUIType;
        
        //Map
        private VisualElement _mapContainer;
        
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
        
        //KeyPad
        private VisualElement _keyPadContainer;
        private Safe _activeSafe;
        private int _keyPadInputLength;
        private string _keyPadInput;

        //=============================== Unity Events ===============================//
        
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            
            _mapContainer = rootVisualElement.Q<VisualElement>("map-view");
             
            _notesContainer = rootVisualElement.Q<VisualElement>("note-view");
            SetupNotes(_notesContainer);
            
            _keyPadContainer = rootVisualElement.Q<VisualElement>("keypad-view");
            SetupKeyPad(_keyPadContainer);
        }

        //=============================== Public Functions ===============================//

        public void OpenNote(Note note, bool standalone = false)
        {
            UIUtils.ShowUIElement(_notesContainer);
            
            if (note.noteType == NoteType.Research)
            {
                UIUtils.HideUIElement(_handWrittenNote);
                UIUtils.ShowUIElement(_researchNote);
                
                _researchNoteContent.text = note.content;
                _researchNoteTitle.text = note.title;
                _researchNoteAuthor.text = note.author;
                _researchNoteDate.text = note.date;
            }
            else
            {
                UIUtils.HideUIElement(_researchNote);
                UIUtils.ShowUIElement(_handWrittenNote);
                
                _handWrittenNoteContent.text = note.content;
                _handWrittenNoteAuthor.text = note.author;
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

        public void OpenMap()
        {
            UIUtils.ShowUIElement(_mapContainer);
            GameManager.Instance.activeWindow = this;
            
            Time.timeScale = 0;
            GameManager.Instance.DisableInput();
            _activeUIType = UIType.Map;
        }
        
        private void CloseMap()
        {
            UIUtils.HideUIElement(_mapContainer);
        }
        
        public void OpenKeyPad(Safe safe)
        {
            UIUtils.ShowUIElement(_keyPadContainer);
            GameManager.Instance.activeWindow = this;
            _activeSafe = safe;
            
            Time.timeScale = 0;
            GameManager.Instance.DisableInput();
            _activeUIType = UIType.KeyPad;
        }
        
        private void CloseKeyPad()
        {
            UIUtils.HideUIElement(_keyPadContainer);
            _activeSafe = null;
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
        }
        
        private void SetupKeyPad(VisualElement keyPadContainer)
        {
            var keysList = keyPadContainer.Query<VisualElement>("keypad-key").ToList();
            
            for(var i = 1; i < 10; i++)
            {
                var key = keysList[i-1];
                var currentKey = i;
                key.RegisterCallback<ClickEvent>(_ =>
                {
                    AddKeyToInput(currentKey);
                });
            }
            
            var zeroKey = keysList[9];
            zeroKey.RegisterCallback<ClickEvent>(_ =>
            {
                AddKeyToInput(0);
            });
            
            var enterKey = keysList[10];
            enterKey.RegisterCallback<ClickEvent>(_ =>
            {
                if (_keyPadInputLength == 4)
                {
                    if (_activeSafe == null) return;

                    if (_keyPadInput == _activeSafe.safeCode)
                    {
                        GameManager.Instance.SoundSystem.Play(keyPadSuccessSound);
                        _activeSafe.OpenSafe();
                        CloseKeyPad();
                    }
                    else ClearKeyPadInput();
                }
                else
                {
                    ClearKeyPadInput();
                }
            });
        }
        
        private void AddKeyToInput(int key)
        {
            if (_keyPadInputLength < 4)
            {
                _keyPadInput += key;
                _keyPadInputLength++;
                GameManager.Instance.SoundSystem.Play(keyPressSound);
            }
            else
            {
                GameManager.Instance.SoundSystem.Play(keyPressErrorSound);
            }
        }
        
        private void ClearKeyPadInput()
        {
            _keyPadInput = "";
            _keyPadInputLength = 0;
            GameManager.Instance.SoundSystem.Play(keyPadErrorSound);
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
