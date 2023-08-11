/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using Runtime.DialogueSystem;

namespace Runtime.Managers
{
    [DefaultExecutionOrder(10)]
    public class TutorialManager : MonoBehaviour
    {
        public CinematicManager cinematicManager;
        public List<Dialogue> tutorialDialogue;
        public bool debug;

        private Dictionary <string, UnityEvent[]> eventDictionary;
        private static TutorialManager _tutorialManager;

        //============================== Unity Events ==============================//
        
        private void Start()
        {
            if (!debug)
            {
                cinematicManager.director.ElementAt(0).Key.time = 0;
                cinematicManager.TriggerCinematic(0);
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

        public static void StartListening(string eventName, UnityAction listener)
        {
            if (Instance.eventDictionary.TryGetValue(eventName, out var thisEvent))
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
                Instance.eventDictionary[eventName] = newEvent;
            } 
            else
            {
                var newEvent = new UnityEvent[1];
                newEvent[0] = new UnityEvent();
                newEvent[0].AddListener(listener);
                Instance.eventDictionary.Add(eventName, newEvent);
            }
        }

        public static void StopListening(string eventName, UnityAction listener)
        {
            if(_tutorialManager == null) return;
            if (Instance.eventDictionary.TryGetValue (eventName, out var thisEvent))
            {
                foreach (var t in thisEvent)
                {
                    t.RemoveListener(listener);
                }
            }
        }

        public static void TriggerEvent(string eventName)
        {
            if (Instance.eventDictionary.TryGetValue (eventName, out var thisEvent))
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
            eventDictionary ??= new Dictionary<string, UnityEvent[]>();
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

        public void TriggerTutorialEnd()
        {
            GameManager.Instance.EndGame();
        }
        
        //============================== Coroutines ==============================//
        
        private IEnumerator Delay()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            GameManager.Instance.DialogueSystem.OnDialogueFinish += TriggerStage4;
        }
    }
}
