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
        [SerializeField] public string persistentID;
        private Dictionary<PlayableDirector, bool> _director;

        private void Start()
        {
            if(!GameManager.Instance.TestMode) TriggerCinematic(0);
        }

        private void Awake()
        {
            _director = GetComponentsInChildren<PlayableDirector>().ToDictionary(playableDirector => playableDirector, _ => false);
        }
        
        public void TriggerCinematic(int index)
        {
            if(index >= _director.Count) Debug.LogError("Index out of range");
            if (!_director.ElementAt(index).Value)
            {
                _director.ElementAt(index).Key.time = 0;
                _director.ElementAt(index).Key.Play();
            }
        }
        
        public string LoadData(SaveGame game)
        {
            if (!game.worldData.cinematics.FindIndex(cinematicData => cinematicData.managerID == persistentID).Equals(-1))
            {
                var cinematicData = game.worldData.cinematics.Find(cinematicData => cinematicData.managerID == persistentID);
                for (var i = 0; i < _director.Count; i++)
                {
                    _director[_director.ElementAt(i).Key] = cinematicData.cinematicStates[i];
                    if(cinematicData.cinematicStates[i]) _director.ElementAt(i).Key.time = _director.ElementAt(i).Key.duration;
                }
            }
            
            return persistentID;
        }

        public void SaveData(SaveGame game)
        {
            if(!game.worldData.cinematics.FindIndex(cinematicData => cinematicData.managerID == persistentID).Equals(-1))
            {
                var cinematicData = game.worldData.cinematics.Find(cinematicData => cinematicData.managerID == persistentID);
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
            
                foreach (var playableDirector in _director)
                {
                    cinematicData.cinematicStates.Add(Math.Abs(playableDirector.Key.time - playableDirector.Key.duration) < 0.01f);
                }
                
                game.worldData.cinematics.Add(cinematicData);
            }
        }
    }
}
