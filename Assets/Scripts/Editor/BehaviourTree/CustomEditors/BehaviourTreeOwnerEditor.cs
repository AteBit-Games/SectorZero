using Runtime.BehaviourTree;
using Runtime.BehaviourTree.Monsters;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.BehaviourTree.CustomEditors 
{
    [CustomEditor(typeof(BehaviourTreeOwner))]
    public class BehaviourTreeInstanceOwner : UnityEditor.Editor
    {
        public VisualTreeAsset mVisualTreeAsset;
        
        public override VisualElement CreateInspectorGUI() 
        {
            var root = new VisualElement();
            mVisualTreeAsset.CloneTree(root);
            
            var openScriptButton = root.Q<Button>("open-button");
            openScriptButton.RegisterCallback<ClickEvent>(_ => CustomInspectorUtils.OpenScriptForEditor(target));

            var foldout = new Foldout
            {
                text = "Default Inspector",
                value = false
            };
            InspectorElement.FillDefaultInspector(foldout, serializedObject, this);
            root.Add(foldout);
            
            return root;
        }
    }
}
