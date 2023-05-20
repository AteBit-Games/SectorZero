/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System.Collections;
using Runtime.InputSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField, Range(0, 1), Tooltip("The percentage of text on the screen, used to simulate the typewriter effect")] private float visibleText;
        [SerializeField, Tooltip("Speed the text is 'typed' onto the screen")] private float textSpeed = 0.05f;
        
        private Label _dialogueTextElement;
        private Label _actorNameTextElement;
        private Label _tapeDateTextElement;
        private VisualElement _actorImage;
        private VisualElement _dialogueContainer;
        private UIDocument _uiDocument;
        
        private float _totalTimeToType, _currentTime;
        private string _currentText;
        
        private Dialogue _currentDialogue;
        private int _currentLineIndex;
        private Coroutine _currentRoutine;
        private bool IsShowing => _dialogueContainer.style.display == DisplayStyle.Flex;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            _dialogueTextElement = rootVisualElement.Q<Label>("dialogue-text");
            _actorNameTextElement = rootVisualElement.Q<Label>("tape-name");
            _tapeDateTextElement = rootVisualElement.Q<Label>("tape-date");
            _actorImage = rootVisualElement.Q<VisualElement>("portrait-image");
            _dialogueContainer = rootVisualElement.Q<VisualElement>("dialogue-window");
        }

        private void OnEnable()
        {
            inputReader.LeftClickEvent += PushText;
        }
        
        private void OnDisable()
        {
            inputReader.LeftClickEvent -= PushText;
        }
        
        public void StartDialogue(Dialogue dialogue)
        {
            Show(true);
            _currentDialogue = dialogue;
            _currentLineIndex = 0;
            _actorNameTextElement.text = dialogue.actor.Name;
            _tapeDateTextElement.text = dialogue.date;
            _actorImage.style.backgroundImage = new StyleBackground(dialogue.actor.Sprite);
            CycleText();
        }
        
        private void Update()
        {
            if(!IsShowing || visibleText >= 1) return;
            _currentTime += Time.deltaTime;
            visibleText = _currentTime / _totalTimeToType;
            UpdateText();
        }
        
        private void UpdateText()
        {
            // Update the text over time to simulate the typewriter effect
            var visibleCharacters = (int)(_currentText.Length * Mathf.Clamp01(visibleText));
            _dialogueTextElement.text = _currentText[..visibleCharacters];
            if (visibleCharacters >= _currentText.Length)
            {
                _dialogueTextElement.text = _currentText;
            }
        }

        private void PushText()
        {
            if(!IsShowing) return;
            
            // If the current line is the last line, hide the dialogue box
            if(visibleText < 1)
            {
                visibleText = 1;
                UpdateText();
                return;
            }
            
            // If the current line is the last line, hide the dialogue box
            if (_currentLineIndex >= _currentDialogue.lines.Count) Show(false);
            else CycleText();
        }

        private void CycleText()
        {
            // Stop the current line being typed
            if(_currentRoutine != null) StopCoroutine(_currentRoutine);
            
            _currentText = _currentDialogue.lines[_currentLineIndex];
            _totalTimeToType = _currentText.Length * textSpeed;
            _currentTime = 0;
            visibleText = 0;
            _dialogueTextElement.text = "";
            _currentLineIndex++;
            
            _currentRoutine = StartCoroutine(WaitForDialogue(_totalTimeToType + 4f));
        }

        private void Show(bool show)
        {
            _dialogueContainer.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }
        
        private IEnumerator WaitForDialogue(float time)
        {
            // Gives the user time to read until the next line is displayed
            yield return new WaitForSeconds(time);
            PushText();
        }
    }
}