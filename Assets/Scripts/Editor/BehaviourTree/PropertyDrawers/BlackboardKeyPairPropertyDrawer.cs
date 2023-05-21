using System.Linq;
using Runtime.BehaviourTree;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.BehaviourTree.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(BlackboardKeyPair))]
    public class BlackboardKeyPairPropertyDrawer : PropertyDrawer
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
            SerializedProperty source = property.FindPropertyRelative(nameof(BlackboardKeyPair.source));
            SerializedProperty target = property.FindPropertyRelative(nameof(BlackboardKeyPair.target));
            
            Runtime.BehaviourTree.BehaviourTree baseTree = GetBehaviourTree(property);
            Runtime.BehaviourTree.BehaviourTree subtree = GetSubtree(property);
            
            if(baseTree == null || subtree == null) return _pairContainer;

            var sourceDropdown = new PopupField<BlackboardKey>
            {
                label = source.displayName,
                formatListItemCallback = FormatItem,
                formatSelectedValueCallback = FormatItem,
                value = source.managedReferenceValue as BlackboardKey
            };
            
            var targetDropdown = new PopupField<BlackboardKey>
            {
                label = target.displayName,
                formatListItemCallback = FormatItem,
                formatSelectedValueCallback = FormatItem,
                value = target.managedReferenceValue as BlackboardKey
            };
            
            sourceDropdown.RegisterCallback<MouseEnterEvent>(_ => {
                sourceDropdown.choices.Clear();
                foreach (var key in baseTree.blackboard.keys) {
                    sourceDropdown.choices.Add(key);
                }
            });
            
            sourceDropdown.RegisterCallback<ChangeEvent<BlackboardKey>>((evt) => {
                BlackboardKey newKey = evt.newValue;
                source.managedReferenceValue = newKey;
                property.serializedObject.ApplyModifiedProperties();
                
                if (_pairContainer.childCount > 1) _pairContainer.Insert(1, targetDropdown);
                else _pairContainer.Add(targetDropdown);
                
                targetDropdown.RegisterCallback<MouseEnterEvent>(_ =>
                {
                    targetDropdown.choices.Clear();
                    //loop through all keys in the subtree and only add the ones that match the type of the source key
                    foreach (var key in subtree.blackboard.keys.Where(key => key.GetType() == sourceDropdown.value.GetType()))
                    {
                        targetDropdown.choices.Add(key);
                    }
                });
                
                targetDropdown.RegisterCallback<ChangeEvent<BlackboardKey>>((evt) =>
                {
                    BlackboardKey newKey = evt.newValue;
                    target.managedReferenceValue = newKey;
                    property.serializedObject.ApplyModifiedProperties();
                });
            });
            
            _pairContainer = new VisualElement();
            _pairContainer.Add(sourceDropdown);
            //
            // if (sourceDropdown.value != null) 
            // {
            //     targetDropdown.RegisterCallback<MouseEnterEvent>(_ =>
            //     {
            //         targetDropdown.choices.Clear();
            //         //loop through all keys in the subtree and only add the ones that match the type of the source key
            //         foreach (var key in subtree.blackboard.keys.Where(key => key.GetType() == sourceDropdown.value.GetType()))
            //         {
            //             targetDropdown.choices.Add(key);
            //         }
            //     });
            //     
            //     targetDropdown.RegisterCallback<ChangeEvent<BlackboardKey>>((evt) =>
            //     {
            //         BlackboardKey newKey = evt.newValue;
            //         target.managedReferenceValue = newKey;
            //         property.serializedObject.ApplyModifiedProperties();
            //     });
            // }
            
            _pairContainer.Add(targetDropdown);
            property.serializedObject.ApplyModifiedProperties();
            return _pairContainer;
        }

        private static string FormatItem(BlackboardKey item)
        {
            return item == null ? "(null)" : item.name;
        }
    }
}
