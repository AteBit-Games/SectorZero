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
    [DefaultExecutionOrder(5)]
    public class Keypad : MonoBehaviour
    {
        //References
        public Sound keyPressSound;
        public Sound keyPadSuccessSound;
        public Sound keyPadErrorSound;
        public Sound keyClearSound;
        
        public KeypadState keypadSuccessState;
        public KeypadState keypadErrorState;
        public List<Key> keysList;
        public Key resetKey;
        public List<Sprite> stateSprites;
        
        //UI
        private List<KeypadKey> _keys = new();
        private VisualElement _errorIcon;
        private VisualElement _successIcon;
        private VisualElement _keypadState;
        
        //References
        private int _keyPadInputLength;
        private int _currentAttempt;
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
                    AddKeyToInput(keyRef.Click());
                    if(_keyPadInputLength == 4)
                    {
                        if (_activeSafe == null) return;
                        _currentAttempt++;

                        if (_keyPadInput == _activeSafe.safeCode)
                        {
                            GameManager.Instance.SoundSystem.Play(keyPadSuccessSound);
                            StartCoroutine(SuccessState());
                        }
                        else ClearKeyPadInput(true);
                        
                        if(_currentAttempt == 3)
                        {
                            GameManager.Instance.HUD.CloseWindow();
                            _activeSafe.TriggerAlarm();
                            _currentAttempt = 0;
                        }
                    }
                });
            }
            
            _errorIcon = keypad.Q<VisualElement>("keypad-error");
            _successIcon = keypad.Q<VisualElement>("keypad-success");
            _keypadState = keypad.Q<VisualElement>("keypad-state");
            
            var resetKeyUi = keypad.Q<VisualElement>("key-reset");
            var resetKeyRef = resetKey;
            _keys.Add(new KeypadKey(resetKeyRef, resetKeyUi, this));    
            
            resetKeyUi.RegisterCallback<ClickEvent>(_ =>
            {
                if (_keyPadInputLength == 4) return;
                _keys[10].Click();
                ClearKeyPadInput(false);
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
                
                UpdateKeyPadState(_keyPadInputLength);
            }
        }

        private void UpdateKeyPadState(int keyPadInputLength)
        {
            _keypadState.style.backgroundImage = new StyleBackground(stateSprites[keyPadInputLength]);
        }

        private void ClearKeyPadInput(bool error)
        {
            _keyPadInput = "";
            _keyPadInputLength = 0;
            UpdateKeyPadState(_keyPadInputLength);
            GameManager.Instance.SoundSystem.Play(error ? keyPadErrorSound : keyClearSound);
            if(error) StartCoroutine(ErrorState());
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
