using System;
using System.Collections.Generic;
using Runtime.BehaviourTree;
using Runtime.BehaviourTree.Composites;
using UnityEditor;
using UnityEngine;

namespace Editor.BehaviourTree 
{
    [Serializable]
    public class SerializedBehaviourTree 
    {
        public SerializedObject serializedObject;
        public Runtime.BehaviourTree.BehaviourTree tree;

        private const string sPropRootNode = nameof(Runtime.BehaviourTree.BehaviourTree.rootNode);
        private const string sPropNodes = nameof(Runtime.BehaviourTree.BehaviourTree.nodes);
        private const string sPropBlackboard = nameof(Runtime.BehaviourTree.BehaviourTree.blackboard);
        private const string sPropBlackboardKeys = nameof(Runtime.BehaviourTree.Blackboard.keys);
        private const string sPropGuid = nameof(Node.guid);
        private const string sPropChild = nameof(DecoratorNode.child);
        private const string sPropChildren = nameof(CompositeNode.children);
        private const string sPropWeights = nameof(ProbabilitySelector.childWeights);
        private const string sPropPosition = nameof(Node.position);
        private const string sViewTransformPosition = nameof(Runtime.BehaviourTree.BehaviourTree.viewPosition);
        private const string sViewTransformScale = nameof(Runtime.BehaviourTree.BehaviourTree.viewScale);
        private bool batchMode;

        public SerializedProperty RootNode => serializedObject.FindProperty(sPropRootNode);

        public SerializedProperty Nodes => serializedObject.FindProperty(sPropNodes);

        public SerializedProperty Blackboard => serializedObject.FindProperty(sPropBlackboard);

        public SerializedProperty BlackboardKeys => serializedObject.FindProperty($"{sPropBlackboard}.{sPropBlackboardKeys}");


        public SerializedBehaviourTree(Runtime.BehaviourTree.BehaviourTree tree)
        {
            serializedObject = new SerializedObject(tree);
            this.tree = tree;
        }

        public SerializedProperty FindNode(SerializedProperty array, Node node) 
        {
            for(int i = 0; i < array.arraySize; ++i)
            {
                var current = array.GetArrayElementAtIndex(i);
                if (current.FindPropertyRelative(sPropGuid).stringValue == node.guid) return current;
            }
            return null;
        }

        public void SetViewTransform(Vector3 position, Vector3 scale) 
        {
            serializedObject.FindProperty(sViewTransformPosition).vector3Value = position;
            serializedObject.FindProperty(sViewTransformScale).vector3Value = scale;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public void SetNodePosition(Node node, Vector2 position) 
        {
            var nodeProp = FindNode(Nodes, node);
            nodeProp.FindPropertyRelative(sPropPosition).vector2Value = position;
            ApplyChanges();
        }

        public void RemoveNodeArrayElement(SerializedProperty array, Node node) 
        {
            for (int i = 0; i < array.arraySize; ++i) 
            {
                var current = array.GetArrayElementAtIndex(i);
                if (current.FindPropertyRelative(sPropGuid).stringValue == node.guid) 
                {
                    array.DeleteArrayElementAtIndex(i);
                    return;
                }
            }
        }

        public Node CreateNodeInstance(Type type) 
        {
            if (Activator.CreateInstance(type) is Node node)
            {
                node.guid = GUID.Generate().ToString();
                return node;
            }
            return null;
        }

        private SerializedProperty AppendArrayElement(SerializedProperty arrayProperty)
        {
            arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
            return arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1);
        }

        public Node CreateNode(Type type, Vector2 position)
        {
            Node child = CreateNodeInstance(type);
            child.position = position;

            SerializedProperty newNode = AppendArrayElement(Nodes);
            newNode.managedReferenceValue = child;

            ApplyChanges();
            return child;
        }

        public void SetRootNode(RootNode node) 
        {
            RootNode.managedReferenceValue = node;
            ApplyChanges();
        }

        public void DeleteNode(Node node) 
        {
            SerializedProperty nodesProperty = Nodes;

            for(int i = 0; i < nodesProperty.arraySize; ++i)
            {
                RemoveNodeArrayElement(Nodes, node);
            }

            ApplyChanges();
        }

        public void AddChild(Node parent, Node child)
        {
            var parentProperty = FindNode(Nodes, parent);
            
            var childProperty = parentProperty.FindPropertyRelative(sPropChild);
            if (childProperty != null) 
            {
                childProperty.managedReferenceValue = child;
                ApplyChanges();
                return;
            }

            var childrenProperty = parentProperty.FindPropertyRelative(sPropChildren);
            if (childrenProperty != null)
            {
                SerializedProperty newChild = AppendArrayElement(childrenProperty);
                newChild.managedReferenceValue = child;

                if (parent is ProbabilitySelector)
                {
                    SerializedProperty weights = parentProperty.FindPropertyRelative(sPropWeights);
                    if (weights.arraySize < childrenProperty.arraySize)
                    {
                        weights.InsertArrayElementAtIndex(weights.arraySize);
                    }
                }

                ApplyChanges();
            }
        }
        
        public void RemoveChild(Node parent, Node child) 
        {
            var parentProperty = FindNode(Nodes, parent);

            var childProperty = parentProperty.FindPropertyRelative(sPropChild);
            if (childProperty != null) 
            {
                childProperty.managedReferenceValue = null;
                ApplyChanges();
                return;
            }
            
            if (parent is ProbabilitySelector probabilitySelector)
            {
                SerializedProperty weights = parentProperty.FindPropertyRelative(sPropWeights);
                weights.DeleteArrayElementAtIndex(probabilitySelector.GetChildIndex(child));
            }

            var childrenProperty = parentProperty.FindPropertyRelative(sPropChildren);
            if (childrenProperty != null) 
            {
                RemoveNodeArrayElement(childrenProperty, child);
                ApplyChanges();
            }
        }

        public void CreateBlackboardKey(string keyName, Type keyType) 
        {
            BlackboardKey key = BlackboardKey.CreateKey(keyType);
            if (key != null) 
            {
                key.name = keyName;
                SerializedProperty keysArray = BlackboardKeys;
                keysArray.InsertArrayElementAtIndex(keysArray.arraySize);
                SerializedProperty newKey = keysArray.GetArrayElementAtIndex(keysArray.arraySize - 1);

                newKey.managedReferenceValue = key;
                ApplyChanges();
            } 
            else 
            {
                Debug.LogError($"Failed to create blackboard key, invalid type:{keyType}");
            }
        }

        public void DeleteBlackboardKey(string keyName) 
        {
            SerializedProperty keysArray = BlackboardKeys;
            for(int i = 0; i < keysArray.arraySize; ++i) 
            {
                var key = keysArray.GetArrayElementAtIndex(i);
                if (key.managedReferenceValue is BlackboardKey itemKey && itemKey.name == keyName)
                {
                    keysArray.DeleteArrayElementAtIndex(i);
                    ApplyChanges();
                    return;
                }
            }
        }

        public void BeginBatch() 
        {
            batchMode = true;
        }

        public void ApplyChanges() 
        {
            if (!batchMode) serializedObject.ApplyModifiedProperties();
        }

        public void EndBatch() 
        {
            batchMode = false;
            ApplyChanges();
        }
    }
}
