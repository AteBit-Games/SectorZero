/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using UnityEngine;

namespace Runtime.Misc.Triggers
{
    public class AnimationTrigger : MonoBehaviour
    {
        public void Trigger(string triggerName)
        {
            var animator = GetComponent<Animator>();
            animator.SetTrigger(triggerName);
        }
    }
}
