/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.InteractionSystem.Objects.UI;
using Runtime.Managers;
using Runtime.SoundSystem;
using Runtime.UI.WindowItems;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.UI
{
    [Serializable]
    public class Key
    {
        public char value;
        public Sprite upIcon;
        public Sprite downIcon;
    }
    
    [Serializable]
    public class KeypadState
    {
        public Sprite onState;
        public Sprite offState;
    }
    
    [Serializable]
    public class Keypad : MonoBehaviour
    {
        //References
        public Sound keyPressSound;
        public Sound keyPressErrorSound;
        public Sound keyPadSuccessSound;
        public Sound keyPadErrorSound;
        
        public KeypadState keypadSuccessState;
        public KeypadState keypadErrorState;
        public List<Key> keysList;
        
        //UI
        private List<KeypadKey> _keys = new();
        private VisualElement _errorIcon;
        private VisualElement _successIcon;
        
        //References
        private int _keyPadInputLength;
        private string _keyPadInput;
        private Safe _activeSafe;
        
        public void SetupKeypad(VisualElement keypad)
        {
            for(var i = 0; i <= 9; i++)
            {
                var uiKey = keypad.Q<VisualElement>("key-" + i);
                _keys.Add(new KeypadKey(keysList[i], uiKey, this));
                
                var keyRef = _keys[i];
                uiKey.RegisterCallback<ClickEvent>(_ =>
                {
                    AddKeyToInput(keyRef.Click(_keyPadInputLength == 4));
                });
            }
            
            _errorIcon = keypad.Q<VisualElement>("keypad-error");
            _successIcon = keypad.Q<VisualElement>("keypad-success");
            
            var enterKey = keypad.Q<VisualElement>("key-submit");
            var enterKeyRef = keysList[10];
            _keys.Add(new KeypadKey(enterKeyRef, enterKey, this));
            
            enterKey.RegisterCallback<ClickEvent>(_ =>
            {
                if (_keyPadInputLength == 4)
                {
                    if (_activeSafe == null) return;

                    if (_keyPadInput == _activeSafe.safeCode)
                    {
                        GameManager.Instance.SoundSystem.Play(keyPadSuccessSound);
                        StartCoroutine(SuccessState());
                    }
                    else ClearKeyPadInput();
                }
                else
                {
                    ClearKeyPadInput();
                }
            });
        }

        public void OpenKeypad(Safe safe)
        {
            _activeSafe = safe;
        }

        //=============================== Public Functions ===============================//
        
        private void AddKeyToInput(char key)
        {
            if (_keyPadInputLength < 4)
            {
                _keyPadInput += key;
                _keyPadInputLength++;
                Debug.Log(_keyPadInput);
            }
        }
   
        private void ClearKeyPadInput()
        {
            _keyPadInput = "";
            _keyPadInputLength = 0;
            GameManager.Instance.SoundSystem.Play(keyPadErrorSound);
            StartCoroutine(ErrorState());
        }
        
        //=============================== Coroutines ===============================//
        
        private IEnumerator SuccessState()
        {
            _successIcon.style.backgroundImage = new StyleBackground(keypadSuccessState.onState);
            yield return new WaitForSecondsRealtime(0.5f);
            GameManager.Instance.HUD.CloseWindow();
            _activeSafe.OpenSafe();
        }
        
        private IEnumerator ErrorState()
        {
            for(var i = 0; i < 2; i++)
            {
                _errorIcon.style.backgroundImage = new StyleBackground(keypadErrorState.onState);
                yield return new WaitForSecondsRealtime(0.2f);
                _errorIcon.style.backgroundImage = new StyleBackground(keypadErrorState.offState);
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
    }
}
