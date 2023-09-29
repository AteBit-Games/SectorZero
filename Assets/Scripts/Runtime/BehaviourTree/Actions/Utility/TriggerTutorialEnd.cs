/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using Runtime.Managers.Tutorial;

namespace Runtime.BehaviourTree.Actions.Utility 
{
    [Serializable]
    [Name("End Tutorial")]
    [Category("Utility")]
    [Description("Helper nod to end the tutorial level")]
    public class TriggerTutorialEnd : ActionNode
    {
        protected override void OnStart()
        {
            TutorialManager.GetCinematicManager().TriggerCinematic(2);
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
