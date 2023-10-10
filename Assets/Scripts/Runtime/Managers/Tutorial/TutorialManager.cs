/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Managers.Tutorial
{
    [DefaultExecutionOrder(6)]
    public class TutorialManager : MonoBehaviour
    {
        public TutorialCinematicManager tutorialCinematicManager;
        public static TutorialCinematicManager GetCinematicManager() => Instance.tutorialCinematicManager;
        
        public List<Dialogue> tutorialDialogue;

        private Dictionary <string, UnityEvent[]> _eventDictionary;
        private static TutorialManager _tutorialManager;

        //============================== Unity Events ==============================//
        
        private void Start()
        {
            if(!GameManager.Instance.TestMode)
            {
                tutorialCinematicManager.director.ElementAt(0).Key.time = 0;
                tutorialCinematicManager.TriggerCinematic(0);
            }
        }

        private void OnEnable()
        {
            StartCoroutine(Delay());
        }

        private void OnDisable()
        {
            GameManager.Instance.DialogueSystem.OnDialogueFinish -= TriggerStage4;
        }
        
        //============================== Public Methods ==============================//

        private static TutorialManager Instance
        {
            get
            {
                if (!_tutorialManager)
                {
                    _tutorialManager = FindFirstObjectByType(typeof(TutorialManager)) as TutorialManager;

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

        public static void StartListening(string eventName, UnityAction listener)
        {
            if (Instance._eventDictionary.TryGetValue(eventName, out var thisEvent))
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
                Instance._eventDictionary[eventName] = newEvent;
            } 
            else
            {
                var newEvent = new UnityEvent[1];
                newEvent[0] = new UnityEvent();
                newEvent[0].AddListener(listener);
                Instance._eventDictionary.Add(eventName, newEvent);
            }
        }

        public static void StopListening(string eventName, UnityAction listener)
        {
            if(_tutorialManager == null) return;
            if (Instance._eventDictionary.TryGetValue (eventName, out var thisEvent))
            {
                foreach (var t in thisEvent)
                {
                    t.RemoveListener(listener);
                }
            }
        }

        public static void TriggerEvent(string eventName)
        {
            if (Instance._eventDictionary.TryGetValue (eventName, out var thisEvent))
            {
                foreach (var t in thisEvent)
                {
                    t.Invoke();
                }
            }
        }
        
        //============================== Private Methods ==============================//
        
        private void Init ()
        {
            _eventDictionary ??= new Dictionary<string, UnityEvent[]>();
        }
        
        //============================== Stage Triggers ==============================//
        
        public void TriggerStage2()
        {
            TriggerEvent("TutorialStage2");
        }

        private void TriggerStage4()
        {
            TriggerEvent("TutorialStage4");
        }
        
        public void TriggerStage5()
        {
            GameManager.Instance.SaveSystem.SaveGame();
        }
        
        public void TriggerDialogue(int index)
        {
            GameManager.Instance.DialogueSystem.StartDialogue(tutorialDialogue[index]);
        }

        public static void TriggerTutorialEnd()
        {
            GameManager.Instance.SaveSystem.SetNellieState("SectorTwo");
            GameManager.Instance.LoadScene("SectorTwo");
        }
        
        //============================== Coroutines ==============================//
        
        private IEnumerator Delay()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            GameManager.Instance.DialogueSystem.OnDialogueFinish += TriggerStage4;
        }
    }
}
