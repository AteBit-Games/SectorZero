/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.GameObject 
{
    [Serializable]
    [Name("Push Back")]
    [Category("GameObject")]
    [Description("Pushes the agent back from the player")]
    public class PushBack : ActionNode
    {
        public NodeProperty<UnityEngine.GameObject> player;

        protected override void OnStart()
        {
            var direction = context.agent.transform.position - player.Value.transform.position;

            var perp = Vector3.Cross(context.transform.forward, direction);
            var dir = Vector3.Dot(perp, player.Value.transform.up);

            var pushDir = dir switch
            {
                <= 0f => Vector3.left,
                > 0f => Vector3.right,
                _ => Vector3.right
            };
            
            context.agent.Warp(context.agent.transform.position + pushDir * 1f);
        }

        protected override void OnStop(){ }
    
        protected override State OnUpdate()
        {
            return State.Success;
        }

        protected override void OnReset() { }
    }
}
