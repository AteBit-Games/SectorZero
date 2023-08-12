/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.InteractionSystem.Objects;
using Runtime.InteractionSystem.Objects.TutorialObjects;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.InteractionSystem 
{
    [CustomEditor(typeof(TutorialCanvas))]
    public class CustomCanvasInspector : UnityEditor.Editor
    {
        public VisualTreeAsset mVisualTreeAsset;
        
        public override VisualElement CreateInspectorGUI() 
        {
            VisualElement root = new VisualElement();
            mVisualTreeAsset.CloneTree(root);
            
            var generateGuidButton = root.Q<Button>("generate-button");
            generateGuidButton.RegisterCallback<ClickEvent>(_ =>
            {
                var canvas = target as TutorialCanvas;
                if (canvas != null && canvas.persistentID == "")
                {
                    canvas.persistentID = System.Guid.NewGuid().ToString();
                    EditorUtility.SetDirty(canvas);
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