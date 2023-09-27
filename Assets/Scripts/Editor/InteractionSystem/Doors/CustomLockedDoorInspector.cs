/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.InteractionSystem.Objects.Doors;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.InteractionSystem.Doors 
{
    [CustomEditor(typeof(LockedDoor))]
    public class CustomLockedDoorInspector : UnityEditor.Editor
    {
        public VisualTreeAsset mVisualTreeAsset;
        
        public override VisualElement CreateInspectorGUI() 
        {
            var root = new VisualElement();
            mVisualTreeAsset.CloneTree(root);
            
            var door = target as LockedDoor;
            if (door == null) return root;
            
            var finishSummaryEntry = root.Q<VisualElement>("finish-summary-entry");
            var addSummaryEntry = root.Q<VisualElement>("add-summary-entry");
            
            var finishSummary = root.Q<PropertyField>("finish-summary");
            finishSummary.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                door.finishSummaryOnOpen = evt.newValue;
                finishSummaryEntry.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                serializedObject.ApplyModifiedProperties();
            });

            var addSummary = root.Q<PropertyField>("add-summary");
            addSummary.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                door.addsSummaryOnFail = evt.newValue;
                addSummaryEntry.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                serializedObject.ApplyModifiedProperties();
            });
            
            addSummaryEntry.style.display = door.addsSummaryOnFail ? DisplayStyle.Flex : DisplayStyle.None;
            finishSummaryEntry.style.display = door.finishSummaryOnOpen ? DisplayStyle.Flex : DisplayStyle.None;
            
            var generateGuidButton = root.Q<Button>("generate-button");
            generateGuidButton.RegisterCallback<ClickEvent>(_ =>
            {
                var door = target as LockedDoor;
                if (door != null && door.persistentID == "")
                {
                    door.persistentID = System.Guid.NewGuid().ToString();
                    EditorUtility.SetDirty(door);
                }
            });

            var openScriptButton = root.Q<Button>("open-button");
            openScriptButton.RegisterCallback<ClickEvent>(_ => CustomInspectorUtils.OpenScriptForEditor(target));
            
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