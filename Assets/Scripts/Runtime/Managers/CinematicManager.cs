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

namespace Runtime.Managers
{
    public class CinematicManager : MonoBehaviour, IPersistant
    {
        [SerializeField] private string persistentID;
        public string ID
        {
            get => persistentID;
            set => persistentID = value;
        }
        
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
        
        public void LoadData(SaveData data)
        {
            if (!data.worldData.cinematics.FindIndex(cinematicData => cinematicData.managerID == persistentID).Equals(-1))
            {
                var cinematicData = data.worldData.cinematics.Find(cinematicData => cinematicData.managerID == persistentID);
                for (var i = 0; i < director.Count; i++)
                {
                    director[director.ElementAt(i).Key] = cinematicData.cinematicStates[i];
                    if(cinematicData.cinematicStates[i]) director.ElementAt(i).Key.time = director.ElementAt(i).Key.duration;
                }
            }
        }

        public void SaveData(SaveData data)
        {
            if(!data.worldData.cinematics.FindIndex(cinematicData => cinematicData.managerID == persistentID).Equals(-1))
            {
                var cinematicData = data.worldData.cinematics.Find(cinematicData => cinematicData.managerID == persistentID);
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
                
                data.worldData.cinematics.Add(cinematicData);
            }
        }
    }
}
