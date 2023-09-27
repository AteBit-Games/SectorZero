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
    [CustomEditor(typeof(Dialogue))]
    public class DialogueInspector : UnityEditor.Editor
    {
        public VisualTreeAsset mVisualTreeAsset;
        
        public override VisualElement CreateInspectorGUI() 
        {
            var root = new VisualElement();
            mVisualTreeAsset.CloneTree(root);
            
            var dialogue = target as Dialogue;
            if (dialogue == null) return root;
            
            var summaryEntry = root.Q<VisualElement>("summary-entry");
            
            var addsSummary = root.Q<PropertyField>("finish-summary");
            addsSummary.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                dialogue.addSummaryEntry = evt.newValue;
                summaryEntry.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                serializedObject.ApplyModifiedProperties();
            });
            
            summaryEntry.style.display = dialogue.addSummaryEntry ? DisplayStyle.Flex : DisplayStyle.None;
            
            return root;
        }
    }
}