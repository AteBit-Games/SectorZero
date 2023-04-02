// Copyright (c) 2023 AteBit Games
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Runtime.SoundSystem.Propagation
{
    [Serializable]
    public class SoundNode : MonoBehaviour
    {
        public int id;
        [SerializeField]
        public List<int> connectionIds;
        
        [NonSerialized]
        public float deltaAngle;
        
        [NonSerialized]
        public float distanceAlongPath;
        
        [NonSerialized]
        public float distanceToTarget;
        
        [NonSerialized]
        public float distanceToParentNode;
        
        [NonSerialized]
        public float volumeReduction;
        
        [NonSerialized]
        public float frequencyReduction;

        [NonSerialized]
        public SoundNode parent;
        
        [NonSerialized]
        public bool visited;
        
        [NonSerialized]
        public float cost;
        
        [NonSerialized]
        public List<SoundNode> connections;
        
        public static int Sort(SoundNode a, SoundNode b)
        {
            return a.cost.CompareTo(b.cost);
        }

        public bool Equals(SoundNode x, SoundNode y)
        {
            return x.id == y.id;
        }

        public int GetHashCode(SoundNode obj)
        {
            return obj.id.GetHashCode();
        }
    }
}