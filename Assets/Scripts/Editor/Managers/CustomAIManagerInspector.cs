/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using Runtime.AI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.Managers 
{
    [CustomEditor(typeof(AIManager))]
    public class CustomAIManagerInspector : UnityEditor.Editor
    {
        public VisualTreeAsset mVisualTreeAsset;
        
        public override VisualElement CreateInspectorGUI() 
        {
            VisualElement root = new VisualElement();
            mVisualTreeAsset.CloneTree(root);
            
            var generateGuidButton = root.Q<Button>("generate-button");
            generateGuidButton.RegisterCallback<ClickEvent>(_ =>
            {
                var aiManager = target as AIManager;
                if (aiManager != null && aiManager.persistentID == "")
                {
                    aiManager.persistentID = System.Guid.NewGuid().ToString();
                    EditorUtility.SetDirty(aiManager);
                }
            });
            
            var debugButton = root.Q<Button>("debug-button");
            debugButton.RegisterCallback<ClickEvent>(_ =>
            {
                var aiManager = target as AIManager;
                if (aiManager != null)
                {
                    aiManager.DebugCloseState = !aiManager.DebugCloseState;
                    EditorUtility.SetDirty(aiManager);
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