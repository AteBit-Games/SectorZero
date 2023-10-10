using System;
using Runtime.Managers;

namespace Runtime.BehaviourTree.Actions.Utility 
{
    [Serializable]
    [Name("Load Scene")]
    [Category("Utility")]
    [Description("Load a scene by index")]
    public class LoadSceneIndex : ActionNode
    {
        public NodeProperty<string> sceneName;

        protected override void OnStart()
        {
            GameManager.Instance.LoadScene(sceneName.Value);
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
