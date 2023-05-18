using System;
using UnityEngine;

namespace Runtime.BehaviourTree.Actions 
{
    [Serializable]
    [Name("Define Search Space")]
    [Category("Navigation")]
    [Description("Create a search space for the monster to search)")]
    public class DefineSearchSpace : ActionNode
    {
        public NodeProperty<Vector2> searchCenter;
        public NodeProperty<float> searchRadius;
        public NodeProperty<Collider2D> outSearchSpace;

        protected override void OnStart()
        {
            //create a circle collider with the search radius and set it to the search center, destroy after 20 seconds
            var searchSpace = new UnityEngine.GameObject("Search Space")
            {
                transform =
                {
                    position = searchCenter.Value
                }
            };
            var circleCollider = searchSpace.AddComponent<CircleCollider2D>();
            circleCollider.radius = searchRadius.Value;
            circleCollider.isTrigger = true;
            UnityEngine.Object.Destroy(searchSpace, 20f);

            outSearchSpace.Value = circleCollider;
        }
    
        protected override void OnStop() { }
    
        protected override State OnUpdate() 
        {
            return State.Success;
        }
        
        protected override void OnReset() { }
    }
}
