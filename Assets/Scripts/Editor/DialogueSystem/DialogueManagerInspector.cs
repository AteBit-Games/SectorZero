/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.DialogueSystem;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.DialogueSystem 
{
    [CustomEditor(typeof(DialogueManager))]
    public class DialogueManagerInspector : UnityEditor.Editor
    {
        public VisualTreeAsset mVisualTreeAsset;
        
        public override VisualElement CreateInspectorGUI() 
        {
            VisualElement root = new VisualElement();
            mVisualTreeAsset.CloneTree(root);
            
            // Default inspector within
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