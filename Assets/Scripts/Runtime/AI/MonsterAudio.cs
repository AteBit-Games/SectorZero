using System.Collections;
using System.Collections.Generic;
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
