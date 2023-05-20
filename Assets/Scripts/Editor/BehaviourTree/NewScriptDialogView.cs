using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.BehaviourTree 
{
    public class NewScriptDialogView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<NewScriptDialogView, UxmlTraits> { }

        private EditorUtility.ScriptTemplate _scriptTemplate;
        private TextField _textField;
        private Button _confirmButton;
        private NodeView _source;
        private bool _isSourceParent;
        private Vector2 _nodePosition;

        public void CreateScript(EditorUtility.ScriptTemplate scriptTemplate, NodeView source, bool isSourceParent, Vector2 position) 
        {
            this._scriptTemplate = scriptTemplate;
            this._source = source;
            this._isSourceParent = isSourceParent;
            _nodePosition = position;

            style.visibility = Visibility.Visible;

            var background = this.Q<VisualElement>("Background");
            var titleLabel = this.Q<Label>("Title");
            _textField = this.Q<TextField>("FileName");
            _confirmButton = this.Q<Button>();

            titleLabel.text = $"New {scriptTemplate.subFolder.TrimEnd('s')} Script";

            _textField.focusable = true;
            RegisterCallback<PointerEnterEvent>(_ => _textField[0].Focus());

            _textField.RegisterCallback<KeyDownEvent>((e) => {
                if (e.keyCode is KeyCode.Return or KeyCode.KeypadEnter) OnConfirm();
            });

            _confirmButton.clicked -= OnConfirm;
            _confirmButton.clicked += OnConfirm;

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
            string scriptName = _textField.text;

            var newNodePath = $"{BehaviourTreeEditorWindow.instance.settings.newNodePath}";
            if (AssetDatabase.IsValidFolder(newNodePath)) 
            {
                var destinationFolder = System.IO.Path.Combine(newNodePath, _scriptTemplate.subFolder);
                var destinationPath = System.IO.Path.Combine(destinationFolder, $"{scriptName}.cs");

                System.IO.Directory.CreateDirectory(destinationFolder);
                var parentPath = System.IO.Directory.GetParent(Application.dataPath);

                string templateString = _scriptTemplate.templateFile.text;
                templateString = templateString.Replace("#SCRIPTNAME#", scriptName);
                if (parentPath != null)
                {
                    string scriptPath = System.IO.Path.Combine(parentPath.ToString(), destinationPath);

                    if (!System.IO.File.Exists(scriptPath)) 
                    {
                        System.IO.File.WriteAllText(scriptPath, templateString);
                    
                        BehaviourTreeEditorWindow.instance.pendingScriptCreate.pendingCreate = true;
                        BehaviourTreeEditorWindow.instance.pendingScriptCreate.scriptName = scriptName;
                        BehaviourTreeEditorWindow.instance.pendingScriptCreate.nodePosition = _nodePosition;
                    
                        if (_source != null) 
                        {
                            BehaviourTreeEditorWindow.instance.pendingScriptCreate.sourceGuid = _source.node.guid;
                            BehaviourTreeEditorWindow.instance.pendingScriptCreate.isSourceParent = _isSourceParent;
                        }

                        AssetDatabase.Refresh();
                        _confirmButton.SetEnabled(false);
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

            _confirmButton.SetEnabled(true);
            Close();
        }
    }
}