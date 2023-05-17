using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.GameObject 
{
    [Serializable]
    [Name("Look Around")]
    [Category("GameObject")]
    [Description("The agent looks around for a while.")]
    public class LookAround : ActionNode
    {
        public NodeProperty<Collider2D> activeRoom;
        public NodeProperty<int> times = new(){Value = 3};
        public NodeProperty<float> lookTime = new(){Value = 2f};
        
        private float _lookTime;
        int _currentTimes;

        protected override void OnStart()
        {
            _lookTime = Time.time;
            _currentTimes = 0;
            activeRoom.Value.bounds.Expand(-5f);
        }

        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            if(_currentTimes >= times.Value)
            {
                return State.Success;
            }

            if (Time.time - _lookTime >= lookTime.Value)
            {
                _lookTime = Time.time;
                var bounds = activeRoom.Value.bounds;
                var x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
                var y = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);

                var target = new Vector2(x, y);
                var direction = target - (Vector2)context.agent.transform.position;

                context.agent.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
                _currentTimes++;
            }

            return State.Running;
        }
        
        protected override void OnReset() { }

        public override void OnDrawGizmos() { }
    }
}
