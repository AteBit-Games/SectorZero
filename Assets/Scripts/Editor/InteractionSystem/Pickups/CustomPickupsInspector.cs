/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using Runtime.InteractionSystem.Items;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.InteractionSystem.Pickups 
{
    [CustomEditor(typeof(Pickup))]
    public class CustomPickupInspector : UnityEditor.Editor
    {
        public VisualTreeAsset mVisualTreeAsset;
        
        public override VisualElement CreateInspectorGUI() 
        {
            VisualElement root = new VisualElement();
            mVisualTreeAsset.CloneTree(root);
            
            var pickup = target as Pickup;
            if (pickup == null) return root;
            
            var summaryEntry = root.Q<VisualElement>("summary-entry");
            
            var finishSummary = root.Q<PropertyField>("finish-summary");
            finishSummary.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                pickup.finishSummaryOnPickup = evt.newValue;
                summaryEntry.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                serializedObject.ApplyModifiedProperties();
            });
            
            summaryEntry.style.display = pickup.finishSummaryOnPickup ? DisplayStyle.Flex : DisplayStyle.None;
            
            var generateGuidButton = root.Q<Button>("generate-button");
            generateGuidButton.RegisterCallback<ClickEvent>(_ =>
            {
                var pickupRef = target as Pickup;
                if (pickupRef != null && pickupRef.persistentID == "")
                {
                    pickupRef.persistentID = System.Guid.NewGuid().ToString();
                    EditorUtility.SetDirty(pickupRef);
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