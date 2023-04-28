using System;
using System.Linq;
using Runtime.BehaviourTree;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.BehaviourTree 
{
    public class BlackboardView : VisualElement 
    {
        public new class UxmlFactory : UxmlFactory<BlackboardView, UxmlTraits> { }
        private SerializedBehaviourTree behaviourTree;

        private ListView listView;
        private TextField newKeyTextField;
        private PopupField<Type> newKeyTypeField;

        private Button createButton;

        internal void Bind(SerializedBehaviourTree behaviourTree) 
        {
            this.behaviourTree = behaviourTree;

            listView = this.Q<ListView>("ListView_Keys");
            newKeyTextField = this.Q<TextField>("TextField_KeyName");
            VisualElement popupContainer = this.Q<VisualElement>("PopupField_Placeholder");
            
            createButton = this.Q<Button>("Button_KeyCreate");
            
            listView.Bind(behaviourTree.serializedObject);

            newKeyTypeField = new PopupField<Type>
            {
                label = "Type",
                formatListItemCallback = FormatItem,
                formatSelectedValueCallback = FormatItem
            };

            var types = TypeCache.GetTypesDerivedFrom<BlackboardKey>();
            foreach (var type in types.Where(type => !type.IsGenericType))
            {
                newKeyTypeField.choices.Add(type);
                newKeyTypeField.value ??= type;
            }
            popupContainer.Clear();
            popupContainer.Add(newKeyTypeField);
            
            newKeyTextField.RegisterCallback<ChangeEvent<string>>(_=> {
                ValidateButton();
            });
            
            createButton.clicked -= CreateNewKey;
            createButton.clicked += CreateNewKey;

            ValidateButton();
        }

        private static string FormatItem(Type arg)
        {
            return arg == null ? "(null)" : arg.Name.Replace("Key", "");
        }

        private void ValidateButton() 
        {
            bool isValidKeyText = ValidateKeyText(newKeyTextField.text);
            createButton.SetEnabled(isValidKeyText);
        }

        private bool ValidateKeyText(string text) 
        {
            if (text == "") return false;

            Runtime.BehaviourTree.BehaviourTree tree = behaviourTree.Blackboard.serializedObject.targetObject as Runtime.BehaviourTree.BehaviourTree;
            bool keyExists = tree != null && tree.blackboard.Find(newKeyTextField.text) != null;
            return !keyExists;
        }

        private void CreateNewKey() 
        {
            Type newKeyType = newKeyTypeField.value;
            if (newKeyType != null)behaviourTree.CreateBlackboardKey(newKeyTextField.text, newKeyType);
            ValidateButton();
        }

        public void ClearView() 
        {
            behaviourTree = null;
            listView?.Unbind();
        }
    }
}