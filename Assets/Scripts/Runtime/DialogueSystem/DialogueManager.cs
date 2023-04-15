/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System.Collections;
using Runtime.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace Runtime.DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textElement;
        [SerializeField] private TextMeshProUGUI actorName;
        [SerializeField] private Image actorImage;
        [SerializeField] private InputReader inputReader;
        
        [Range(0, 1)] 
        [SerializeField] private float visibleText;
        [SerializeField] private float textSpeed = 0.05f;
        private float _totalTimeToType, _currentTime;
        private string _currentText;
        
        private Dialogue _currentDialogue;
        private int _currentLineIndex;
        private Coroutine _currentRoutine;
        bool IsShowing => gameObject.activeSelf;

        private void Start()
        {
            inputReader.LeftClickEvent += PushText;
        }
        
        public void StartDialogue(Dialogue dialogue)
        {
            Show(true);
            _currentDialogue = dialogue;
            _currentLineIndex = 0;
            actorName.text = dialogue.actor.Name;
            actorImage.sprite = dialogue.actor.Sprite;
            CycleText();
        }
        
        private void Update()
        {
            if(!IsShowing) return;
            if (visibleText >= 1) return;
            _currentTime += Time.deltaTime;
            visibleText = _currentTime / _totalTimeToType;
            UpdateText();
        }
        
        private void UpdateText()
        {
            var visibleCharacters = (int)(_currentText.Length * Mathf.Clamp01(visibleText));
            textElement.text = _currentText[..visibleCharacters];
            if (visibleCharacters >= _currentText.Length)
            {
                textElement.text = _currentText;
            }
        }

        private void PushText()
        {
            if(visibleText < 1)
            {
                visibleText = 1;
                UpdateText();
                return;
            }
            
            if(_currentLineIndex >= _currentDialogue.lines.Count) Show(false);
            else CycleText();
        }
        
        private void CycleText()
        {
            if(_currentRoutine != null) StopCoroutine(_currentRoutine);
            
            _currentText = _currentDialogue.lines[_currentLineIndex];
            _totalTimeToType = _currentText.Length * textSpeed;
            _currentTime = 0;
            visibleText = 0;
            textElement.text = "";
            _currentLineIndex++;
            
            _currentRoutine = StartCoroutine(WaitForDialogue(_totalTimeToType + 2f));
        }

        private void Show(bool show)
        {
            gameObject.SetActive(show);
        }
        
        private IEnumerator WaitForDialogue(float time)
        {
            yield return new WaitForSeconds(time);
            PushText();
        }
    }
}