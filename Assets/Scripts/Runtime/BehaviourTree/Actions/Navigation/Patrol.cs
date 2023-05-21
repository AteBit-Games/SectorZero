using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.Navigation 
{
    [Serializable]
    [Name("Patrol")]
    [Category("Navigation")]
    public class Patrol : ActionNode
    {
        public enum PatrolMode
        {
            Progressive,
            Random
        }
        
        public PatrolMode patrolMode = PatrolMode.Random;
        public NodeProperty<List<Collider2D>> targetList;
        public NodeProperty<float> speed = new(){Value = 4f};
        public NodeProperty<float> keepDistance = new(){Value = 0.1f};
        public NodeProperty<Collider2D> outRoom;

        private int _index = -1;
        private Vector3? _lastRequest;

        protected override void OnStart()
        {
            if(targetList.Value.Count == 1) 
            {
                _index = 0;
            }
            else
            {
                switch (patrolMode)
                {
                    case PatrolMode.Random:
                    {
                        var newIndex = _index;
                        while (newIndex == _index)
                        {
                            newIndex = UnityEngine.Random.Range(0, targetList.Value.Count);
                        }
                        _index = newIndex;
                        break;
                    }
                    case PatrolMode.Progressive:
                        _index = (int)Mathf.Repeat(_index + 1, targetList.Value.Count);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            outRoom.Value = targetList.Value[_index];
            context.agent.speed = speed.Value;
            context.agent.isStopped = false;
        }

        protected override void OnStop()
        {
            if(_lastRequest != null) context.agent.isStopped = true;
            _lastRequest = null;
        }
    
        protected override State OnUpdate() 
        {
            var target = targetList.Value[_index].transform.position;
            if (_lastRequest != target)
            {
                if (!context.agent.SetDestination(target))
                {
                    return State.Failure;
                }
            }
            
            _lastRequest = target;
            return (context.agent.transform.position - target).magnitude < context.agent.stoppingDistance + keepDistance.Value ? State.Success : State.Running;
        }
        
        protected override void OnReset() { }
        
        public override void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            foreach (var target in targetList.Value)
            {
                Gizmos.color = targetList.Value.IndexOf(target) == _index ? new Color(0f, 0.4f, 1f) : new Color(0.19f, 0.87f, 1f);
                Gizmos.DrawSphere(target.transform.position, 0.5f);
            }
        }
    }
}
