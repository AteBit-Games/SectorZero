/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.SoundSystem.ScriptableObjects;
using UnityEditor;
using UnityEngine.UIElements;

namespace Editor.SoundSystem 
{
    [CustomEditor(typeof(Sound))]
    public class SoundInspector : UnityEditor.Editor
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