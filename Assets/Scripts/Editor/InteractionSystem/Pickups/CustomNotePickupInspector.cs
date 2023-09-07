/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using Runtime.InteractionSystem.Items;
using Runtime.InventorySystem.ScriptableObjects;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.InteractionSystem.Pickups 
{
    [CustomEditor(typeof(NotePickup))]
    public class CustomNotePickupInspector : UnityEditor.Editor
    {
        public VisualTreeAsset mVisualTreeAsset;
        
        public override VisualElement CreateInspectorGUI() 
        {
            VisualElement root = new VisualElement();
            mVisualTreeAsset.CloneTree(root);
            
            var generateGuidButton = root.Q<Button>("generate-button");
            generateGuidButton.RegisterCallback<ClickEvent>(_ =>
            {
                var pickup = target as NotePickup;
                if (pickup != null && pickup.persistentID == "")
                {
                    pickup.persistentID = System.Guid.NewGuid().ToString();
                    EditorUtility.SetDirty(pickup);
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