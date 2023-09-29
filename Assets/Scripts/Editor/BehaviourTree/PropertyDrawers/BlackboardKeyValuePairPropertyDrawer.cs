using Runtime.BehaviourTree;
using Runtime.BehaviourTree.Monsters;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.BehaviourTree.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(BlackboardKeyValuePair))]
    public class BlackboardKeyValuePairPropertyDrawer : PropertyDrawer 
    {
        private VisualElement pairContainer;

        static Runtime.BehaviourTree.BehaviourTree GetBehaviourTree(SerializedProperty property)
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

        public override VisualElement CreatePropertyGUI(SerializedProperty property) 
        {
            SerializedProperty first = property.FindPropertyRelative(nameof(BlackboardKeyValuePair.key));
            SerializedProperty second = property.FindPropertyRelative(nameof(BlackboardKeyValuePair.value));

            PopupField<BlackboardKey> dropdown = new PopupField<BlackboardKey>();
            dropdown.label = first.displayName;
            dropdown.formatListItemCallback = FormatItem;
            dropdown.formatSelectedValueCallback = FormatItem;
            dropdown.value = first.managedReferenceValue as BlackboardKey;
            
            Runtime.BehaviourTree.BehaviourTree tree = GetBehaviourTree(property);
            dropdown.RegisterCallback<MouseEnterEvent>((evt) => {
                dropdown.choices.Clear();
                foreach (var key in tree.blackboard.keys) {
                    dropdown.choices.Add(key);
                }
            });

            dropdown.RegisterCallback<ChangeEvent<BlackboardKey>>((evt) => {
                BlackboardKey newKey = evt.newValue;
                first.managedReferenceValue = newKey;
                property.serializedObject.ApplyModifiedProperties();

                if (pairContainer.childCount > 1) pairContainer.RemoveAt(1);

                if (second.managedReferenceValue == null || second.managedReferenceValue.GetType() != dropdown.value.GetType()) 
                {
                    second.managedReferenceValue = BlackboardKey.CreateKey(dropdown.value.GetType());
                    second.serializedObject.ApplyModifiedProperties();
                }
                
                PropertyField field = new PropertyField();
                field.label = second.displayName;
                field.BindProperty(second.FindPropertyRelative(nameof(BlackboardKey<object>.value)));
                pairContainer.Add(field);
            });

            pairContainer = new VisualElement();
            pairContainer.Add(dropdown);

            if (dropdown.value != null) 
            {
                if (second.managedReferenceValue == null || first.managedReferenceValue.GetType() != second.managedReferenceValue.GetType()) 
                {
                    second.managedReferenceValue = BlackboardKey.CreateKey(dropdown.value.GetType());
                    second.serializedObject.ApplyModifiedProperties();
                }

                PropertyField field = new PropertyField();
                field.label = second.displayName;
                field.bindingPath = nameof(BlackboardKey<object>.value);
                pairContainer.Add(field);
            }

            return pairContainer;
        }

        private static string FormatItem(BlackboardKey item)
        {
            return item == null ? "(null)" : item.name;
        }
    }
}
