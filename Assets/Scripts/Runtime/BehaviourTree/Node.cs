using System;
using UnityEngine;

namespace Runtime.BehaviourTree 
{
    [Serializable]
    public abstract class Node 
    {
        public enum State 
        {
            Inactive,
            Running,
            Failure,
            Success
        }

        [HideInInspector] public State state = State.Inactive;
        [HideInInspector] public bool started;
        [HideInInspector] public string guid = Guid.NewGuid().ToString();
        [HideInInspector] public Vector2 position;

        public Context context;
        [HideInInspector] public Blackboard blackboard;
        [TextArea] public string comment;
        [Tooltip("When enabled, the nodes OnDrawGizmos will be invoked")] public bool drawGizmos;

        private string _description;
        private string _category;
        
        public string Description
        {
            get
            {
                if (_description == null)
                {
                    var attribute = (DescriptionAttribute) Attribute.GetCustomAttribute(GetType(), typeof(DescriptionAttribute));
                    _description = attribute?.description ?? "";
                }
                return _description;
            }
        }
        
        public string Category
        {
            get
            {
                if (_category == null)
                {
                    var attribute = (CategoryAttribute) Attribute.GetCustomAttribute(GetType(), typeof(CategoryAttribute));
                    _category = attribute?.category ?? "";
                }
                return _category;
            }
        }
        
        public string Name
        {
            get
            {
                var attribute = (NameAttribute) Attribute.GetCustomAttribute(GetType(), typeof(NameAttribute));
                return attribute?.name ?? GetType().Name;
            }
        }
        
        public virtual void OnInit() { }

        public State Update() 
        {
            if (!started) 
            {
                OnStart();
                started = true;
            }

            state = OnUpdate();

            if (state != State.Running)
            {
                OnStop();
                started = false;
            }
            return state;
        }

        public void Abort() 
        {
            BehaviourTree.Traverse(this, node => {
                node.started = false;
                node.state = State.Running;
                node.OnStop();
            });
        }
        
        public void Reset()
        {
            BehaviourTree.Traverse(this, node =>
            {
                node.OnReset();
            });
        }
        
        public virtual void OnDrawGizmos() { }

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();
        protected abstract void OnReset();
    }
}