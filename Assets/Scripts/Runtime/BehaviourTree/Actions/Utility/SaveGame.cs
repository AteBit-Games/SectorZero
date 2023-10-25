using System;
using System.Collections;
using Runtime.Managers;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions.Utility 
{
    [Serializable]
    [Name("Save Game")]
    [Description("Save the game")]
    [Category("Utility")]
    public class SaveGame : ActionNode
    {
        public float delay = 0.5f;

        protected override void OnStart()
        {
            GameManager.Instance.StartCoroutine(SaveCoroutine());
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            return State.Success;
        }
        
        protected override void OnReset() { }
        
        private IEnumerator SaveCoroutine()
        {
            yield return new WaitForSeconds(delay);
            GameManager.Instance.SaveSystem.SaveGame();
        }
    }
}
