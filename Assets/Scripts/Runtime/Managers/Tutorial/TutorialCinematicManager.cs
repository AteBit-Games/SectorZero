/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using UnityEngine;
using UnityEngine.Playables;

namespace Runtime.Managers.Tutorial
{
    public class TutorialCinematicManager : MonoBehaviour, IPersistant
    {
        [SerializeField] public string persistentID;
        
        public Dictionary<PlayableDirector, bool> director;

        private void Awake()
        {
            director = GetComponentsInChildren<PlayableDirector>().ToDictionary(playableDirector => playableDirector, _ => false);
        }
        
        public void TriggerCinematic(int index)
        {
            if(index >= director.Count) Debug.LogError("Index out of range");
            if (!director.ElementAt(index).Value)
            {
                director.ElementAt(index).Key.Play();
            }
        }
        
        public string LoadData(SaveGame game)
        {
            if (!game.tutorialData.tutorialCinematics.FindIndex(cinematicData => cinematicData.managerID == persistentID).Equals(-1))
            {
                var cinematicData = game.tutorialData.tutorialCinematics.Find(cinematicData => cinematicData.managerID == persistentID);
                for (var i = 0; i < director.Count; i++)
                {
                    director[director.ElementAt(i).Key] = cinematicData.cinematicStates[i];
                    if(cinematicData.cinematicStates[i]) director.ElementAt(i).Key.time = director.ElementAt(i).Key.duration;
                }
            }
            
            return persistentID;
        }

        public void SaveData(SaveGame game)
        {
            if(!game.tutorialData.tutorialCinematics.FindIndex(cinematicData => cinematicData.managerID == persistentID).Equals(-1))
            {
                var cinematicData = game.tutorialData.tutorialCinematics.Find(cinematicData => cinematicData.managerID == persistentID);
                for (var i = 0; i < director.Count; i++)
                {
                    cinematicData.cinematicStates[i] = Math.Abs(director.ElementAt(i).Key.time - director.ElementAt(i).Key.duration) < 0.01f;
                }
            }
            else
            {
                var cinematicData = new CinematicData
                {
                    managerID = persistentID,
                    cinematicStates = new List<bool>()
                };

                foreach (var playableDirector in director)
                {
                    cinematicData.cinematicStates.Add(Math.Abs(playableDirector.Key.time - playableDirector.Key.duration) < 0.01f);
                }
                
                game.tutorialData.tutorialCinematics.Add(cinematicData);
            }
        }
    }
}
