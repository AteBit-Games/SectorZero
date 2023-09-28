using System;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.BehaviourTree.Conditions.Navigation
{
    [Serializable]
    [Name("Is Point Valid")]
    [Category("Navigation")]
    [Description("Check if the agent has a valid path")]
    public class ValidPoint : ConditionNode
    {
        public NodeProperty<Vector2> positionToCheck;

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            //var isValid = NavMesh.SamplePosition(positionToCheck.Value, out _, 0.4f, NavMesh.AllAreas);
            
            //set destination and see if the destination is on the position or a partial position
            if(context.agent.SetDestination(positionToCheck.Value))
            {
                var path = context.agent.path;
                var pathLength = path.corners.Length;
                Debug.Log($"Path Length: {pathLength}");
                Debug.Log($"Path Status: {path.corners}");
                //Debug.Log($"Agent Destination: {path.corners[pathLength - 1]}");
            }
            
            //Debug.Log($"Valid Point: {isValid}");
            
            //return isValid ? State.Success : State.Failure;

            return State.Failure;
        }

        protected override void OnReset() { }
    }
}