using Runtime.BehaviourTree;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.BehaviourTree.CustomEditors 
{
    [CustomEditor(typeof(BehaviourTreeOwner))]
    public class BehaviourTreeInstanceEditor : UnityEditor.Editor 
    {
        public override VisualElement CreateInspectorGUI() 
        {
            VisualElement container = new VisualElement();

            PropertyField treeField = new PropertyField();
            treeField.bindingPath = nameof(BehaviourTreeOwner.behaviourTree);

            PropertyField validateField = new PropertyField();
            validateField.bindingPath = nameof(BehaviourTreeOwner.validate);

            PropertyField publicKeys = new PropertyField();
            publicKeys.bindingPath = nameof(BehaviourTreeOwner.blackboardOverrides);

            container.Add(treeField);
            container.Add(validateField);
            container.Add(publicKeys);

            return container;
        }
    }
}
