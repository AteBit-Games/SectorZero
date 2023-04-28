using System;
using System.Linq;
using Runtime.BehaviourTree;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.BehaviourTree.PropertyDrawers {

    [CustomPropertyDrawer(typeof(NodeProperty<>))]
    public class GenericNodePropertyPropertyDrawer : PropertyDrawer 
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property) 
        {
            Runtime.BehaviourTree.BehaviourTree tree = property.serializedObject.targetObject as Runtime.BehaviourTree.BehaviourTree;

            var genericTypes = fieldInfo.FieldType.GenericTypeArguments;
            var propertyType = genericTypes[0];

            SerializedProperty reference = property.FindPropertyRelative("reference");

            Label label = new Label();
            label.AddToClassList("unity-base-field__label");
            label.AddToClassList("unity-property-field__label");
            label.AddToClassList("unity-property-field");
            label.text = property.displayName;

            PropertyField defaultValueField = new PropertyField();
            defaultValueField.label = "";
            defaultValueField.style.flexGrow = 1.0f;
            defaultValueField.bindingPath = nameof(NodeProperty<int>.defaultValue);

            var dropdown = new PopupField<BlackboardKey>
            {
                label = "",
                formatListItemCallback = FormatItem,
                formatSelectedValueCallback = FormatSelectedItem,
                value = reference.managedReferenceValue as BlackboardKey,
                tooltip = "Bind value to a BlackboardKey",
                style =
                {
                    flexGrow = 1.0f
                }
            };
            
            dropdown.RegisterCallback<MouseEnterEvent>((evt) => {
                dropdown.choices.Clear();
                foreach (var key in tree.blackboard.keys.Where(key => propertyType.IsAssignableFrom(key.underlyingType)))
                {
                    dropdown.choices.Add(key);
                }
                dropdown.choices.Add(null);

                dropdown.choices.Sort((left, right) => {
                    if (left == null) {
                        return -1;
                    }
                    return right == null ? 1 : string.Compare(left.name, right.name, StringComparison.Ordinal);
                });
            });

            dropdown.RegisterCallback<ChangeEvent<BlackboardKey>>((evt) => {
                BlackboardKey newKey = evt.newValue;
                reference.managedReferenceValue = newKey;
                BehaviourTreeEditorWindow.Instance.serializer.ApplyChanges();

                if (evt.newValue == null) 
                {
                    defaultValueField.style.display = DisplayStyle.Flex;
                    dropdown.style.flexGrow = 0.0f;
                } 
                else 
                {
                    defaultValueField.style.display = DisplayStyle.None;
                    dropdown.style.flexGrow = 1.0f;
                }
            });

            defaultValueField.style.display = dropdown.value == null ? DisplayStyle.Flex : DisplayStyle.None;
            dropdown.style.flexGrow = dropdown.value == null ? 0.0f : 1.0f;

            VisualElement container = new VisualElement();
            container.AddToClassList("unity-base-field");
            container.AddToClassList("node-property-field");
            container.style.flexDirection = FlexDirection.Row;
            container.Add(label);
            container.Add(defaultValueField);
            container.Add(dropdown);

            return container;
        }

        private static string FormatItem(BlackboardKey item)
        {
            return item == null ? "[Inline]" : item.name;
        }

        private static string FormatSelectedItem(BlackboardKey item)
        {
            return item == null ? "" : item.name;
        }
    }

    [CustomPropertyDrawer(typeof(NodeProperty))]
    public class NodePropertyPropertyDrawer : PropertyDrawer 
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property) 
        {
            Runtime.BehaviourTree.BehaviourTree tree = property.serializedObject.targetObject as Runtime.BehaviourTree.BehaviourTree;

            SerializedProperty reference = property.FindPropertyRelative("reference");

            var dropdown = new PopupField<BlackboardKey>
            {
                label = property.displayName,
                formatListItemCallback = FormatItem,
                formatSelectedValueCallback = FormatItem,
                value = reference.managedReferenceValue as BlackboardKey
            };

            dropdown.RegisterCallback<MouseEnterEvent>((evt) => {
                dropdown.choices.Clear();
                foreach (var key in tree.blackboard.keys) {
                    dropdown.choices.Add(key);
                }
                dropdown.choices.Sort((left, right) => string.Compare(left.name, right.name, StringComparison.Ordinal));
            });

            dropdown.RegisterCallback<ChangeEvent<BlackboardKey>>((evt) => {
                BlackboardKey newKey = evt.newValue;
                reference.managedReferenceValue = newKey;
                BehaviourTreeEditorWindow.Instance.serializer.ApplyChanges();
            });
            
            return dropdown;
        }

        private static string FormatItem(BlackboardKey item)
        {
            return item == null ? "(null)" : item.name;
        }
    }
}