using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Runtime.BehaviourTree;
using Runtime.BehaviourTree.Monsters;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.BehaviourTree.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(BlackboardSubKey))]
    public class BlackboardKeyPairProperty : PropertyDrawer
    {
        private VisualElement _pairContainer;

        private static Runtime.BehaviourTree.BehaviourTree GetBehaviourTree(SerializedProperty property)
        {
            switch (property.serializedObject.targetObject)
            {
                case Runtime.BehaviourTree.BehaviourTree tree:
                    return tree;
                case BehaviourTreeOwner instance:
                    return instance.behaviourTree;
                default:
                    Debug.LogError("Could not find behaviour tree this is referencing");
                    return null;
            }
        }
        
        private static Runtime.BehaviourTree.BehaviourTree GetSubtree(SerializedProperty property)
        {
            SerializedBehaviourTree serializer = BehaviourTreeEditorWindow.instance.serializer;
            SerializedProperty node = null;
            foreach (SerializedProperty item in serializer.Nodes)
            {
                if (property.propertyPath.Contains(item.propertyPath))
                {
                    node = item;
                    break;
                }
            }
            
            SerializedProperty treeAsset = node.FindPropertyRelative(nameof(SubTree.treeAsset));
            return treeAsset.objectReferenceValue as Runtime.BehaviourTree.BehaviourTree;
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _pairContainer = new VisualElement();
            
            var source = property.FindPropertyRelative(nameof(BlackboardSubKey.source));
            var target = property.FindPropertyRelative(nameof(BlackboardSubKey.target));
            
            var baseTree = GetBehaviourTree(property);
            var subtree = GetSubtree(property);
            
            if(baseTree == null || subtree == null) return _pairContainer;

            var targetDropdown = new PopupField<BlackboardKey>
            {
                label = "",
                formatListItemCallback = FormatItem,
                formatSelectedValueCallback = FormatSelectedTargetItem,
                value = target.managedReferenceValue as BlackboardKey,
            };
                
            targetDropdown.RegisterCallback<MouseEnterEvent>(_ => {
                targetDropdown.choices.Clear();
                foreach (var key in subtree.blackboard.keys)
                {
                    targetDropdown.choices.Add(key);
                }
                targetDropdown.choices.Add(null);
                
                targetDropdown.choices.Sort((left, right) => {
                    if (left == null) {
                        return -1;
                    }
                    return right == null ? 1 : string.Compare(left.name, right.name, StringComparison.Ordinal);
                });
            });
                
            targetDropdown.RegisterCallback<ChangeEvent<BlackboardKey>>(evt => {
                BlackboardKey newKey = evt.newValue;
                target.managedReferenceValue = newKey;
                BehaviourTreeEditorWindow.instance.serializer.ApplyChanges();
            });
            
            VisualElement sourceValueContainer = new();
            if(target.managedReferenceValue != null)
            {
                Label label = new Label();
                label.AddToClassList("unity-base-field__label");
                label.AddToClassList("unity-property-field__label");
                label.AddToClassList("unity-property-field");
                label.text = "Source Value";

                PropertyField sourceValueField = new PropertyField(target);
                sourceValueField.label = "";
                sourceValueField.style.flexGrow = 1.0f;
                sourceValueField.AddToClassList("target-input");
                sourceValueField.bindingPath = "target";

                var sourceDropdown = new PopupField<BlackboardKey>
                {
                    label = "",
                    formatListItemCallback = FormatItem,
                    formatSelectedValueCallback = FormatSelectedSourceItem,
                    value = source.managedReferenceValue as BlackboardKey,
                    tooltip = "Bind value to a BlackboardKey",
                    style =
                    {
                        flexGrow = 1.0f
                    }
                };
                
                sourceDropdown.RegisterCallback<MouseEnterEvent>(_ => {
                    sourceDropdown.choices.Clear();
                    foreach (var key in baseTree.blackboard.keys.Where(key => key.GetType() == target.managedReferenceValue.GetType()))
                    {
                        sourceDropdown.choices.Add(key);
                    }
                    sourceDropdown.choices.Add(null);

                    sourceDropdown.choices.Sort((left, right) => {
                        if (left == null) {
                            return -1;
                        }
                        return right == null ? 1 : string.Compare(left.name, right.name, StringComparison.Ordinal);
                    });
                });

                sourceDropdown.RegisterCallback<ChangeEvent<BlackboardKey>>(evt => {
                    BlackboardKey newKey = evt.newValue;
                    source.managedReferenceValue = newKey;
                    BehaviourTreeEditorWindow.instance.serializer.ApplyChanges();
                });
                
                
                if (sourceDropdown.value == null) sourceValueContainer.Add(label);
                sourceDropdown.style.flexGrow = sourceDropdown.value == null ? 0.0f : 1.0f;
                
                sourceValueField.style.display = sourceDropdown.value == null ? DisplayStyle.Flex : DisplayStyle.None;
                sourceValueContainer.AddToClassList("unity-base-field");
                sourceValueContainer.AddToClassList("node-property-field");
                sourceValueContainer.style.flexDirection = FlexDirection.Row;
                
                sourceValueContainer.Add(sourceValueField);
                sourceValueContainer.Add(sourceDropdown);
            }
            
            _pairContainer.Add(targetDropdown);
            _pairContainer.Add(sourceValueContainer);

            return _pairContainer;
        }
        
        private static string FormatSelectedTargetItem(BlackboardKey item)
        {
            return item == null ? "[No Key Selected]" : item.name;
        }
        
        private static string FormatSelectedSourceItem(BlackboardKey item)
        {
            return item == null ? "" : item.name;
        }
        
        private static string FormatItem(BlackboardKey item)
        {
            return item == null ? "(null)" : item.name;
        }
    }
}
