using Runtime.BehaviourTree;
using UnityEditor;
using UnityEngine.UIElements;

namespace Editor.BehaviourTree.CustomEditors 
{
    [CustomEditor(typeof(BehaviourTreeOwner))]
    public class BehaviourTreeInstanceOwner : UnityEditor.Editor
    {
        public VisualTreeAsset mVisualTreeAsset;
        
        public override VisualElement CreateInspectorGUI() 
        {
            VisualElement root = new VisualElement();
            mVisualTreeAsset.CloneTree(root);
            return root;
        }
    }
}
