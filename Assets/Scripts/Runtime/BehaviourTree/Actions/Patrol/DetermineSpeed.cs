/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;

namespace Runtime.BehaviourTree.Actions.Patrol 
{
    [Serializable]
    [Name("Determine Speed")]
    [Category("Patrol")]
    [Description("Determine the speed of the agent based on the aggro level")]
    public class DetermineSpeed : ActionNode
    {
        public NodeProperty<int> aggroLevel;
        public NodeProperty<float> outSpeed;

        protected override void OnStart()
        {
            switch (aggroLevel.Value)
            {
                case <= 3:
                    outSpeed.Value = 4f;
                    break;
                case <= 6:
                    outSpeed.Value = 5f;
                    break;
                case <= 9:
                    outSpeed.Value = 5.5f;
                    break;
                case 10:
                    outSpeed.Value = 6f;
                    break;
            }
            
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
