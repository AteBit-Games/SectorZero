using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.BehaviourTree 
{
    public class OverlayView : VisualElement 
    {
        public new class UxmlFactory : UxmlFactory<OverlayView, UxmlTraits> { }
        public System.Action<Runtime.BehaviourTree.BehaviourTree> OnTreeSelected;

        private Button openButton;
        private Button createButton;
        private DropdownField assetSelector;
        private TextField treeNameField;
        private TextField locationPathField;

        public void Show() 
        {
            var settings = new SerializedObject(BehaviourTreeEditorWindow.instance.settings);
            style.visibility = Visibility.Visible;

            // Configure fields
            openButton = this.Q<Button>("OpenButton");
            assetSelector = this.Q<DropdownField>();
            createButton = this.Q<Button>("CreateButton");
            treeNameField = this.Q<TextField>("TreeName");
            locationPathField = this.Q<TextField>("LocationPath");
            
            parent.ClearClassList();
            parent.AddToClassList("overlay-active");

            locationPathField.BindProperty(settings.FindProperty("newTreePath"));
            
            var behaviourTrees = EditorUtility.GetAssetPaths<Runtime.BehaviourTree.BehaviourTree>();
            assetSelector.choices = new List<string>();
            behaviourTrees.ForEach(treePath => {
                assetSelector.choices.Add(ToMenuFormat(treePath));
            });
            
            openButton.clicked -= OnOpenAsset;
            openButton.clicked += OnOpenAsset;
            
            createButton.clicked -= OnCreateAsset;
            createButton.clicked += OnCreateAsset;
        }

        public void Hide() 
        {
            style.visibility = Visibility.Hidden;
            parent.ClearClassList();
            parent.AddToClassList("overlay-inactive");
        }

        private static string ToMenuFormat(string one) 
        {
            return one.Replace("/", "|");
        }

        private static string ToAssetFormat(string one) 
        {
            return one.Replace("|", "/");
        }

        private void OnOpenAsset() 
        {
            string path = ToAssetFormat(assetSelector.text);
            Runtime.BehaviourTree.BehaviourTree tree = AssetDatabase.LoadAssetAtPath<Runtime.BehaviourTree.BehaviourTree>(path);
            
            if(tree) 
            {
                TreeSelected(tree);
                style.visibility = Visibility.Hidden;
            }
        }

        private void OnCreateAsset() 
        {
            Runtime.BehaviourTree.BehaviourTree tree = EditorUtility.CreateNewTree(treeNameField.text, locationPathField.text);
            if (tree)
            {
                TreeSelected(tree);
                style.visibility = Visibility.Hidden;
            }
        }

        private void TreeSelected(Runtime.BehaviourTree.BehaviourTree tree) 
        {
            OnTreeSelected.Invoke(tree);
        }
    }
}
