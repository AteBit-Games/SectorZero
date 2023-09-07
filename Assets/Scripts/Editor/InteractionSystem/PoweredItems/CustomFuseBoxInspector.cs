/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using Runtime.InteractionSystem.Objects.Doors;
using Runtime.InteractionSystem.Objects.Powered;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.InteractionSystem.PoweredItems 
{
    [CustomEditor(typeof(FuseBox))]
    public class CustomFuseBoxInspector : UnityEditor.Editor
    {
        public VisualTreeAsset mVisualTreeAsset;
        
        public override VisualElement CreateInspectorGUI() 
        {
            VisualElement root = new VisualElement();
            mVisualTreeAsset.CloneTree(root);
            
            var fuseBox = target as FuseBox;
            if (fuseBox == null) return root;
            
            var fuseSound = root.Q<VisualElement>("fuse-sound");
            var fuseObject = root.Q<PropertyField>("fuse-object");
            
            var hasFuseField = root.Q<PropertyField>("start-fuse");
            hasFuseField.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                fuseBox.startWithFuse = evt.newValue;
                if(!evt.newValue)
                {
                    fuseSound.style.display = DisplayStyle.Flex;
                    fuseObject.style.display = DisplayStyle.Flex;
                }
                else
                {
                    fuseSound.style.display = DisplayStyle.None;
                    fuseObject.style.display = DisplayStyle.None;
                }
                serializedObject.ApplyModifiedProperties();
            });
            
            var generateGuidButton = root.Q<Button>("generate-button");
            generateGuidButton.RegisterCallback<ClickEvent>(_ =>
            {
                var fuseBox = target as FuseBox;
                if (fuseBox != null && fuseBox.persistentID == "")
                {
                    fuseBox.persistentID = System.Guid.NewGuid().ToString();
                    EditorUtility.SetDirty(fuseBox);
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