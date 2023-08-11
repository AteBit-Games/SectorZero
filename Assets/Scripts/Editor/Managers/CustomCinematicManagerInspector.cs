/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.Managers;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.Managers 
{
    [CustomEditor(typeof(CinematicManager))]
    public class CustomCinematicManagerInspector : UnityEditor.Editor
    {
        public VisualTreeAsset mVisualTreeAsset;
        
        public override VisualElement CreateInspectorGUI() 
        {
            VisualElement root = new VisualElement();
            mVisualTreeAsset.CloneTree(root);
            
            var generateGuidButton = root.Q<Button>("generate-button");
            generateGuidButton.RegisterCallback<ClickEvent>(_ =>
            {
                var cinematicManager = target as CinematicManager;
                if (cinematicManager != null && cinematicManager.ID == "")
                {
                    cinematicManager.ID = System.Guid.NewGuid().ToString();
                    EditorUtility.SetDirty(cinematicManager);
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