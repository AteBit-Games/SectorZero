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
    [CustomEditor(typeof(Actor))]
    public class ActorInspector : UnityEditor.Editor
    {
        public VisualTreeAsset mVisualTreeAsset;
        
        public override VisualElement CreateInspectorGUI() 
        {
            var root = new VisualElement();
            mVisualTreeAsset.CloneTree(root);
            
            return root;
        }
    }
}