/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections;
using Runtime.InputSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Runtime.DialogueSystem
{
    [DefaultExecutionOrder(2)]
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField, Tooltip("Speed the text is 'typed' onto the screen")] private float textSpeed = 0.05f;
        
        //============== Settings ================
        public event Action OnDialogueFinish;
        private AudioSource _audioSource;
        
        //================ UI Elements ==================
        private Label _dialogueTextElement;
        private Label _actorNameTextElement;
        private Label _dateTextElement;
        private VisualElement _actorImage;
        private VisualElement _dialogueContainer;
        private UIDocument _uiDocument;

        //================ Dialogue ==================
        private Dialogue _currentDialogue;
        private bool _skipDialogue;
        
        //================ Tracking ==================
        private int _currentLineIndex;
        private Coroutine displayLineCoroutine;

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
            
            //Bind Audio Source
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        private void OnEnable()
        {
            inputReader.LeftClickEvent += SkipDialogue;
        }
        
        private void OnDisable()
        {
            inputReader.LeftClickEvent -= SkipDialogue;
        }

        private void SkipDialogue()
        {
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
            
            ContinueDialogue();
        }

        private void ContinueDialogue() 
        {
            if(_currentDialogue == null) return;
            
            if (_currentLineIndex < _currentDialogue.dialogueLines.Count)
            {
                _skipDialogue = false;
                
                // set text for the current dialogue line
                if (displayLineCoroutine != null) 
                {
                    StopCoroutine(displayLineCoroutine);
                }
                displayLineCoroutine = StartCoroutine(DisplayLine(_currentDialogue, _currentLineIndex));
                _currentLineIndex++;
            }
            else
            {
                if (_skipDialogue) ShowDialogue(false);
                else StartCoroutine(ExitDialogue());

                if(_currentDialogue.trigger) OnDialogueFinish?.Invoke();
                _currentDialogue = null;
            }
        }

        private IEnumerator DisplayLine(Dialogue dialogue, int lineIndex)
        {
            var currentLetterIndex = 0;
            _actorNameTextElement.text = dialogue.dialogueLines[_currentLineIndex].actor.Name;
            _actorImage.style.backgroundImage = new StyleBackground(dialogue.dialogueLines[_currentLineIndex].actor.Sprite);
            
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
                yield return new WaitForSeconds(textSpeed);
            }
            
            _dialogueTextElement.text = dialogue.dialogueLines[lineIndex].line;
            yield return new WaitForSeconds(2f);
            ContinueDialogue();
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

                    var minPitchHash = (int) actor.MinPitch * 100;
                    var maxPitchHash = (int) actor.MaxPitch * 100;
                    var pitchRange = maxPitchHash - minPitchHash;

                    if (pitchRange == 0)
                    {
                        _audioSource.pitch = actor.MinPitch;
                    }
                    else
                    {
                        var pitchHash = hash % pitchRange + minPitchHash;
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
        
        private IEnumerator ExitDialogue() 
        {
            yield return new WaitForSeconds(2f);
            ShowDialogue(false);
        }
        
        private void ShowDialogue(bool show)
        {
            _dialogueContainer.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}