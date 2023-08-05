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
        
        private Dictionary<PlayableDirector, bool> _director;

        private void Awake()
        {
            _director = GetComponentsInChildren<PlayableDirector>().ToDictionary(director => director, _ => false);
        }
        
        public void TriggerCinematic(int index)
        {
            if(index >= _director.Count) Debug.LogError("Index out of range");
            if (!_director.ElementAt(index).Value)
            {
                _director.ElementAt(index).Key.Play();
            }
        }
        
        public void LoadData(SaveData data)
        {
            if (!data.worldData.cinematics.FindIndex(cinematicData => cinematicData.managerID == persistentID).Equals(-1))
            {
                var cinematicData = data.worldData.cinematics.Find(cinematicData => cinematicData.managerID == persistentID);
                for (var i = 0; i < _director.Count; i++)
                {
                    _director[_director.ElementAt(i).Key] = cinematicData.cinematicStates[i];
                    if(cinematicData.cinematicStates[i]) _director.ElementAt(i).Key.time = _director.ElementAt(i).Key.duration;
                }
            }
        }

        public void SaveData(SaveData data)
        {
            if(!data.worldData.cinematics.FindIndex(cinematicData => cinematicData.managerID == persistentID).Equals(-1))
            {
                var cinematicData = data.worldData.cinematics.Find(cinematicData => cinematicData.managerID == persistentID);
                for (var i = 0; i < _director.Count; i++)
                {
                    cinematicData.cinematicStates[i] = Math.Abs(_director.ElementAt(i).Key.time - _director.ElementAt(i).Key.duration) < 0.01f;
                }
            }
            else
            {
                var cinematicData = new CinematicData
                {
                    managerID = persistentID,
                    cinematicStates = new List<bool>()
                };

                foreach (var director in _director)
                {
                    cinematicData.cinematicStates.Add(Math.Abs(director.Key.time - director.Key.duration) < 0.01f);
                }
                
                data.worldData.cinematics.Add(cinematicData);
            }
        }
    }
}
