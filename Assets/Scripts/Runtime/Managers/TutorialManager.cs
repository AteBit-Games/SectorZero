/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Runtime.DialogueSystem;

namespace Runtime.Managers
{
    public class TutorialManager : MonoBehaviour
    {
        public List<Dialogue> tutorialDialogue;
        
        private Dictionary <string, UnityEvent[]> eventDictionary;
        private static TutorialManager _tutorialManager;

        private static TutorialManager instance
        {
            get
            {
                if (!_tutorialManager)
                {
                    _tutorialManager = FindObjectOfType(typeof(TutorialManager)) as TutorialManager;

                    if (!_tutorialManager)
                    {
                        Debug.LogError ("There needs to be one active EventManger script on a GameObject in your scene.");
                    }
                    else
                    {
                        _tutorialManager.Init(); 
                    }
                }

                return _tutorialManager;
            }
        }

        private void Init ()
        {
            eventDictionary ??= new Dictionary<string, UnityEvent[]>();
        }

        public static void StartListening(string eventName, UnityAction listener)
        {
            if (instance.eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                var oldLength = thisEvent.Length;
                var newLength = oldLength + 1;
                var newEvent = new UnityEvent[newLength];
                for (var i = 0; i < oldLength; i++)
                {
                    newEvent[i] = thisEvent[i];
                }

                newEvent[newLength - 1] = new UnityEvent();
                newEvent[newLength - 1].AddListener(listener);
                instance.eventDictionary[eventName] = newEvent;
            } 
            else
            {
                var newEvent = new UnityEvent[1];
                newEvent[0] = new UnityEvent();
                newEvent[0].AddListener(listener);
                instance.eventDictionary.Add(eventName, newEvent);
            }
        }

        public static void StopListening(string eventName, UnityAction listener)
        {
            if(_tutorialManager == null) return;
            if (instance.eventDictionary.TryGetValue (eventName, out var thisEvent))
            {
                foreach (var t in thisEvent)
                {
                    t.RemoveListener(listener);
                }
            }
        }

        public static void TriggerEvent(string eventName)
        {
            if (instance.eventDictionary.TryGetValue (eventName, out var thisEvent))
            {
                foreach (var t in thisEvent)
                {
                    t.Invoke();
                }
            }
        }
        
        public void TriggerStage2()
        {
            TriggerEvent("TutorialStage2");
        }
        
        public void TriggerDialogue(int index)
        {
            GameManager.Instance.DialogueSystem.StartDialogue(tutorialDialogue[index]);
        }
    }
}