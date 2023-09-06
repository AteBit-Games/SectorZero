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
            VisualElement root = new VisualElement();
            mVisualTreeAsset.CloneTree(root);
            
            var note = target as Note;
            if (note == null) return root;
            
            //callback for when the note type is changed
            var noteTypeField = root.Q<PropertyField>("note-type");
            noteTypeField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                Enum.TryParse(evt.newValue, out NoteType valueInt);
                note.noteType = valueInt;
                root.Q<VisualElement>("extra-settings").style.display = valueInt switch
                {
                    NoteType.Handwritten => DisplayStyle.None,
                    NoteType.Research => DisplayStyle.Flex,
                    _ => throw new ArgumentOutOfRangeException()
                };

                serializedObject.ApplyModifiedProperties();
            });

            if (note.noteType == NoteType.Handwritten)
            {
                root.Q<VisualElement>("extra-settings").style.display = DisplayStyle.None;
            }

            return root;
        }
    }
}