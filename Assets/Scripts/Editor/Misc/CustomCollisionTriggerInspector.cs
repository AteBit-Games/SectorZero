/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.Misc.Triggers;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Misc 
{
    [CustomEditor(typeof(CollisionTrigger))]
    public class CustomCollisionTriggerInspector : UnityEditor.Editor
    {
        public VisualTreeAsset mVisualTreeAsset;
        
        public override VisualElement CreateInspectorGUI() 
        {
            VisualElement root = new VisualElement();
            mVisualTreeAsset.CloneTree(root);
            
            var generateGUIDButton = root.Q<Button>("generate-button");
            generateGUIDButton.RegisterCallback<ClickEvent>(_ =>
            {
                var trigger = target as CollisionTrigger;
                if (trigger != null && trigger.ID == "")
                {
                    trigger.ID = System.Guid.NewGuid().ToString();
                    EditorUtility.SetDirty(trigger);
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