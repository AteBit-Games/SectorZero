/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
using Runtime.Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.UI.WindowItems
{
    public class KeypadKey 
    {
        //UI
        private readonly VisualElement _key;
        
        //References
        private readonly Keypad _keyPadRef;
        private readonly Key _keyRef;
        private Coroutine _pressCoroutine;
        
        public KeypadKey(Key key, VisualElement keypadKeyRef, Keypad keypadRef)
        {
            _keyRef = key;
            _keyPadRef = keypadRef;
            _key = keypadKeyRef;
            
            _key.style.backgroundImage = new StyleBackground(key.upIcon);
        }

        public char Click()
        {
            if(_pressCoroutine != null) GameManager.Instance.StopCoroutine(_pressCoroutine);
            _pressCoroutine = _keyPadRef.StartCoroutine(Press());

            GameManager.Instance.SoundSystem.Play(_keyPadRef.keyPressSound);
            return _keyRef.value;
        }

        private IEnumerator Press()
        {
            _key.style.backgroundImage = new StyleBackground(_keyRef.downIcon);
            yield return new WaitForSecondsRealtime(0.1f);
            _key.style.backgroundImage = new StyleBackground(_keyRef.upIcon);
        }
    }
}