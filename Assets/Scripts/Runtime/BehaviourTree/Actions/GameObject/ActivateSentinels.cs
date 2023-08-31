/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections.Generic;
using Runtime.AI;

namespace Runtime.BehaviourTree.Actions.GameObject 
{
    [Serializable]
    [Name("Activate Sentinels")]
    [Category("GameObject")]
    [Description("Activate sentinels based on parameters")]
    public class ActivateSentinels : ActionNode
    {
        public NodeProperty<List<UnityEngine.GameObject>> sentinelList;
        public NodeProperty<float> sentinelDuration;
        public NodeProperty<int> sentinelCount;

        private List<UnityEngine.GameObject> _selectedSentinels = new();
        
        protected override void OnStart()
        {
            var sentinels = new List<UnityEngine.GameObject>(sentinelList.Value);
            _selectedSentinels.Clear();
            
            for (var i = 0; i < sentinelCount.Value; i++)
            {
                var index = UnityEngine.Random.Range(0, sentinels.Count);
                _selectedSentinels.Add(sentinels[index]);
                sentinels.RemoveAt(index);
            }
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            foreach (var sentinel in _selectedSentinels)
            {
                sentinel.GetComponentInChildren<Sentinel>().ActivateSentinel(sentinelDuration.Value);
            }
            
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}


