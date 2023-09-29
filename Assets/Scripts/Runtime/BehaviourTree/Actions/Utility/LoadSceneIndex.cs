using System;
using Runtime.Managers;
using UnityEngine.SceneManagement;

namespace Runtime.BehaviourTree.Actions.Utility 
{
    [Serializable]
    [Name("Load Scene Index")]
    [Category("Utility")]
    [Description("Load a scene by index")]
    public class LoadSceneIndex : ActionNode
    {
        public NodeProperty<int> sceneIndex;

        protected override void OnStart()
        {
            GameManager.Instance.LoadScene(sceneIndex.Value);
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
