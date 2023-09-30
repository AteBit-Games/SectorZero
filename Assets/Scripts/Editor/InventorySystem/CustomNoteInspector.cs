/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using Runtime.InventorySystem.ScriptableObjects;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.InventorySystem 
{
    [CustomEditor(typeof(Note))]
    public class CustomNoteInspector : UnityEditor.Editor
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