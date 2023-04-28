using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.BehaviourTree 
{
    public class NewScriptDialogView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<NewScriptDialogView, UxmlTraits> { }

        private EditorUtility.ScriptTemplate scriptTemplate;
        private TextField textField;
        private Button confirmButton;
        private NodeView source;
        private bool isSourceParent;
        private Vector2 nodePosition;

        public void CreateScript(EditorUtility.ScriptTemplate scriptTemplate, NodeView source, bool isSourceParent, Vector2 position) 
        {
            this.scriptTemplate = scriptTemplate;
            this.source = source;
            this.isSourceParent = isSourceParent;
            nodePosition = position;

            style.visibility = Visibility.Visible;

            var background = this.Q<VisualElement>("Background");
            var titleLabel = this.Q<Label>("Title");
            textField = this.Q<TextField>("FileName");
            confirmButton = this.Q<Button>();

            titleLabel.text = $"New {scriptTemplate.subFolder.TrimEnd('s')} Script";

            textField.focusable = true;
            RegisterCallback<PointerEnterEvent>(_ => textField[0].Focus());

            textField.RegisterCallback<KeyDownEvent>((e) => {
                if (e.keyCode is KeyCode.Return or KeyCode.KeypadEnter) OnConfirm();
            });

            confirmButton.clicked -= OnConfirm;
            confirmButton.clicked += OnConfirm;

            background.RegisterCallback<PointerDownEvent>((e) => {
                e.StopImmediatePropagation(); 
                Close();
            });
        }

        private void Close() 
        {
            style.visibility = Visibility.Hidden;
        }

        private void OnConfirm() 
        {
            string scriptName = textField.text;

            var newNodePath = $"{BehaviourTreeEditorWindow.Instance.settings.newNodePath}";
            if (AssetDatabase.IsValidFolder(newNodePath)) 
            {
                var destinationFolder = System.IO.Path.Combine(newNodePath, scriptTemplate.subFolder);
                var destinationPath = System.IO.Path.Combine(destinationFolder, $"{scriptName}.cs");

                System.IO.Directory.CreateDirectory(destinationFolder);
                var parentPath = System.IO.Directory.GetParent(Application.dataPath);

                string templateString = scriptTemplate.templateFile.text;
                templateString = templateString.Replace("#SCRIPTNAME#", scriptName);
                if (parentPath != null)
                {
                    string scriptPath = System.IO.Path.Combine(parentPath.ToString(), destinationPath);

                    if (!System.IO.File.Exists(scriptPath)) 
                    {
                        System.IO.File.WriteAllText(scriptPath, templateString);
                    
                        BehaviourTreeEditorWindow.Instance.pendingScriptCreate.pendingCreate = true;
                        BehaviourTreeEditorWindow.Instance.pendingScriptCreate.scriptName = scriptName;
                        BehaviourTreeEditorWindow.Instance.pendingScriptCreate.nodePosition = nodePosition;
                    
                        if (source != null) 
                        {
                            BehaviourTreeEditorWindow.Instance.pendingScriptCreate.sourceGuid = source.node.guid;
                            BehaviourTreeEditorWindow.Instance.pendingScriptCreate.isSourceParent = isSourceParent;
                        }

                        AssetDatabase.Refresh();
                        confirmButton.SetEnabled(false);
                        EditorApplication.delayCall += WaitForCompilation;
                    } 
                    else 
                    {
                        Debug.LogError($"Script with that name already exists:{scriptPath}");
                        Close();
                    }
                }
            }
        }

        private void WaitForCompilation()
        {
            if (EditorApplication.isCompiling)
            {
                EditorApplication.delayCall += WaitForCompilation;
                return;
            }

            confirmButton.SetEnabled(true);
            Close();
        }
    }
}