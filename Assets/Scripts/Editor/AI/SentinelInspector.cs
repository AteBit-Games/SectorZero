using Runtime.AI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.AI 
{
    [CustomEditor(typeof(Sentinel))]
    public class SentinelInspector : UnityEditor.Editor
    {
        public VisualTreeAsset mVisualTreeAsset;
        
        public override VisualElement CreateInspectorGUI() 
        {
            VisualElement root = new VisualElement();
            mVisualTreeAsset.CloneTree(root);
            
            // // Default inspector within
            // var foldout = new Foldout { text = "Default Inspector" };
            // InspectorElement.FillDefaultInspector(foldout, serializedObject, this);
            // root.Add(foldout);

            return root;
        }
    }
}