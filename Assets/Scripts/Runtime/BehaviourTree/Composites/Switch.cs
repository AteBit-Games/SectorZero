using UnityEngine.Serialization;

namespace Runtime.BehaviourTree.Composites 
{
    [System.Serializable]
    [Name("Switch")]
    [Category("Composites")]
    [Description("Executes one child based on the provided int or enum and returns its status.")]
    public class Switch : CompositeNode
    {
        public NodeProperty<int> index;
        public bool interruptible = true;
        private int _currentIndex;
        
        protected override void OnStart()
        {
            _currentIndex = index.Value;
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (interruptible) 
            {
                int nextIndex = index.Value;
                if (nextIndex != _currentIndex) children[_currentIndex].Abort();
                _currentIndex = nextIndex;
            }

            return _currentIndex < children.Count ? children[_currentIndex].Update() : State.Failure;
        }

        protected override void OnReset()
        {
            _currentIndex = 0;
        }
    }
}

