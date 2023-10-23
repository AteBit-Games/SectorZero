/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections;
using System.Linq;
using Runtime.Managers;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Runtime.DialogueSystem
{
    [DefaultExecutionOrder(3)]
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField, Tooltip("Speed the text is 'typed' onto the screen")] private float textSpeed = 0.05f;
        [SerializeField] private float sentencePauseTime;
        
        //============== Settings ================
        public event Action OnDialogueFinish;
        private AudioSource _audioSource;
        
        //================ UI Elements ==================
        private Label _dialogueTextElement;
        private Label _actorNameTextElement;
        private Label _dateTextElement;
        private VisualElement _clickIcon;
        
        private VisualElement _actorImage;
        private VisualElement _dialogueContainer;
        private UIDocument _uiDocument;

        //================ Dialogue ==================
        private Dialogue _currentDialogue;
        private bool _skipDialogue;
        
        //================ Tracking ==================
        private int _currentLineIndex;
        private Coroutine _displayLineCoroutine;
        private Coroutine _endDialogueCoroutine;
        private readonly char[] _sentenceBreakCharacters = { '.', '!', '?', '"' };
        
        //============================== Unity Events ==============================//
        
        private void Awake()
        {
            //Bind UI Elements
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            _dialogueTextElement = rootVisualElement.Q<Label>("dialogue-text");
            _actorNameTextElement = rootVisualElement.Q<Label>("tape-name");
            _dateTextElement = rootVisualElement.Q<Label>("tape-date");
            _actorImage = rootVisualElement.Q<VisualElement>("portrait-image");
            _dialogueContainer = rootVisualElement.Q<VisualElement>("dialogue-window");
            _clickIcon = rootVisualElement.Q<VisualElement>("click-image");
            
            //Bind Audio Source
            _audioSource = gameObject.GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            GameManager.Instance.inputReader.LeftClickEvent += SkipDialogue;
        }
        
        private void OnDisable()
        {
            GameManager.Instance.inputReader.LeftClickEvent -= SkipDialogue;
        }
        
        //============================== Dialogue Functions ==============================//

        private void SkipDialogue()
        {
            if(_currentDialogue == null) return;
            if(!_currentDialogue.canSkip) return;
            
            if(_skipDialogue) ContinueDialogue();
            else _skipDialogue = true;
        }
        
        public void StartDialogue(Dialogue dialogue)
        {
            ShowDialogue(true);
            
            _currentDialogue = dialogue;
            _currentLineIndex = 0;
            _skipDialogue = false;

            _actorNameTextElement.text = dialogue.dialogueLines[_currentLineIndex].actor.Name;
            _dateTextElement.text = dialogue.date;
            _actorImage.style.backgroundImage = new StyleBackground(dialogue.dialogueLines[_currentLineIndex].actor.Sprite);
            _dialogueTextElement.text = "";
            _clickIcon.style.display = _currentDialogue.canSkip ? DisplayStyle.Flex : DisplayStyle.None;
            
            if (_displayLineCoroutine != null) StopCoroutine(_displayLineCoroutine);
            if (_endDialogueCoroutine != null) StopCoroutine(_endDialogueCoroutine);

            ContinueDialogue();
        }

        private void ContinueDialogue() 
        {
            if(_currentDialogue == null) return;
            
            if (_currentLineIndex < _currentDialogue.dialogueLines.Count)
            {
                _skipDialogue = false;
                
                // set text for the current dialogue line
                if (_displayLineCoroutine != null) 
                {
                    StopCoroutine(_displayLineCoroutine);
                }
                _displayLineCoroutine = StartCoroutine(DisplayLine(_currentDialogue, _currentLineIndex));
                _currentLineIndex++;
            }
            else
            {
                if (_skipDialogue) ShowDialogue(false);
                else _endDialogueCoroutine = StartCoroutine(ExitDialogue());

                if(_currentDialogue.trigger) OnDialogueFinish?.Invoke();
                if (_currentDialogue.addSummaryEntry)
                {
                    var inventory = GameManager.Instance.InventorySystem.PlayerInventory;
                    foreach (var entry in _currentDialogue.summaryEntry.Where(entry => !inventory.ContainsSummaryEntry(entry)))
                    {
                        inventory.AddSummaryEntry(entry);
                    }
                }
                
                _currentDialogue = null;
            }
        }
        
        private void PlayDialogueSound(Actor actor, int currentLetterIndex, char character)
        {
            if (currentLetterIndex % actor.Frequency == 0)
            {
                if(actor.StopAudioSource) _audioSource.Stop();

                AudioClip soundClip;
                if (actor.UseHash)
                {
                    var hash = character.GetHashCode();
                    
                    soundClip = actor.ActorSounds[hash % actor.ActorSounds.Count];

                    var minPitchHash = (int) (actor.MinPitch * 100);
                    var maxPitchHash = (int) (actor.MaxPitch * 100);
                    var pitchRange = maxPitchHash - minPitchHash;

                    if (pitchRange == 0)
                    {
                        _audioSource.pitch = actor.MinPitch;
                    }
                    else
                    {
                        var pitchHash = (hash % pitchRange) + minPitchHash;
                        _audioSource.pitch = pitchHash / 100f;
                    }
                }
                else
                {
                    _audioSource.pitch = Random.Range(actor.MinPitch, actor.MaxPitch);
                    soundClip = actor.ActorSounds[Random.Range(0, actor.ActorSounds.Count)];
                }

                _audioSource.PlayOneShot(soundClip);
            }
        }
        
        //============================== UI Functions ==============================//
        
        private void ShowDialogue(bool show)
        {
            if(show) UIUtils.ShowUIElement(_dialogueContainer);
            else UIUtils.HideUIElement(_dialogueContainer);
        }
        
        public void CancelDialogue()
        {
            if (_displayLineCoroutine != null) StopCoroutine(_displayLineCoroutine);
            if (_endDialogueCoroutine != null) StopCoroutine(_endDialogueCoroutine);
            ShowDialogue(false);
            _currentDialogue = null;
        }
        
        //============================== Coroutines ==============================//
        
        private IEnumerator DisplayLine(Dialogue dialogue, int lineIndex)
        {
            var currentLetterIndex = 0;
            _actorNameTextElement.text = dialogue.dialogueLines[_currentLineIndex].actor.Name;
            _actorImage.style.backgroundImage = new StyleBackground(dialogue.dialogueLines[_currentLineIndex].actor.Sprite);

            var previousLetter = new char();
            foreach(var character in dialogue.dialogueLines[lineIndex].line)
            {
                // finish up displaying the line right away if left click is pressed
                if(_skipDialogue) break;

                //Add a letter to the dialogue text
                var text = dialogue.dialogueLines[lineIndex].line[..currentLetterIndex];
                _dialogueTextElement.text = text;
                
                //Play a sound
                PlayDialogueSound(dialogue.dialogueLines[lineIndex].actor, currentLetterIndex, character);
                currentLetterIndex++;

                if (_sentenceBreakCharacters.Contains(previousLetter) && character == ' ') yield return new WaitForSeconds(sentencePauseTime);
                else yield return new WaitForSeconds(textSpeed);
                previousLetter = character;
            }
            
            _dialogueTextElement.text = dialogue.dialogueLines[lineIndex].line;

            if(GameManager.Instance.SaveSystem.GetPlayerData().autoSkip)
            {
                if(!_skipDialogue) yield return new WaitForSeconds(_currentDialogue.autoSkipDelay);
                else yield return new WaitForSeconds(DetermineDisplayTime(dialogue.dialogueLines[lineIndex].line));
                
                ContinueDialogue();
            }
        }
        
        private float DetermineDisplayTime(string text)
        {
            return text.Length * textSpeed + 4f;
        }
        
        private IEnumerator ExitDialogue() 
        {
            yield return new WaitForSeconds(2f);
            ShowDialogue(false);
        }
    }
}