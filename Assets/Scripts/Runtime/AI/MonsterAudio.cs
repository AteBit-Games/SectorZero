/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.BehaviourTree;
using UnityEngine;

public class MonsterAudio : MonoBehaviour
{
    private BehaviourTreeOwner _behaviourTreeOwner;
    
    private void Awake()
    {
        _behaviourTreeOwner = GetComponentInParent<BehaviourTreeOwner>();
    }
    
    public void PlayFootstepSound()
    {
        _behaviourTreeOwner.PlayFootstepSound();
    }
}
