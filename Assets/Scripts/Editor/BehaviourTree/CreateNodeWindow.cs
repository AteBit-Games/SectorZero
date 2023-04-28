using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.BehaviourTree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.BehaviourTree
{
    public class CreateNodeWindow : ScriptableObject, ISearchWindowProvider 
    {
        private Texture2D _icon;
        private BehaviourTreeView _treeView;
        private NodeView _source;
        private bool _isSourceParent;
        private EditorUtility.ScriptTemplate[] _scriptFileAssets;

        private static TextAsset GetScriptTemplate(int type) 
        {
            var projectSettings = BehaviourTreeProjectSettings.GetOrCreateSettings();

            switch (type) {
                case 0:
                    return projectSettings.scriptTemplateActionNode ? projectSettings.scriptTemplateActionNode : BehaviourTreeEditorWindow.Instance.scriptTemplateActionNode;
                case 1:
                    return projectSettings.scriptTemplateCompositeNode ? projectSettings.scriptTemplateCompositeNode : BehaviourTreeEditorWindow.Instance.scriptTemplateCompositeNode;
                case 2:
                    return projectSettings.scriptTemplateDecoratorNode ? projectSettings.scriptTemplateDecoratorNode : BehaviourTreeEditorWindow.Instance.scriptTemplateDecoratorNode;
                case 3:
                    return projectSettings.scriptTemplateConditionNode ? projectSettings.scriptTemplateConditionNode : BehaviourTreeEditorWindow.Instance.scriptTemplateConditionNode;
            }
            Debug.LogError("Unhandled script template type:" + type);
            return null;
        }

        private void Initialise(BehaviourTreeView treeView, NodeView source, bool isSourceParent) 
        {
            _treeView = treeView;
            _source = source;
            _isSourceParent = isSourceParent;

            _icon = new Texture2D(1, 1);
            _icon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _icon.Apply();

            _scriptFileAssets = new[] {
                new EditorUtility.ScriptTemplate { templateFile = GetScriptTemplate(0), defaultFileName = "NewActionNode", subFolder = "Actions" },
                new EditorUtility.ScriptTemplate { templateFile = GetScriptTemplate(1), defaultFileName = "NewCompositeNode", subFolder = "Composites" },
                new EditorUtility.ScriptTemplate { templateFile = GetScriptTemplate(2), defaultFileName = "NewDecoratorNode", subFolder = "Decorators" },
                new EditorUtility.ScriptTemplate { templateFile = GetScriptTemplate(3), defaultFileName = "NewConditionNode", subFolder = "Conditions" },
            };
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context) 
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node")),
            };
            

            if (_isSourceParent || _source == null)
            {
                tree.Add(new SearchTreeGroupEntry(new GUIContent("Actions")) { level = 1 });
                var types = new List<Type>(TypeCache.GetTypesDerivedFrom<ActionNode>());
                types = types.OrderBy(t => t.GetCustomAttributes(typeof(CategoryAttribute), false).Length > 0 ? ((CategoryAttribute)t.GetCustomAttributes(typeof(CategoryAttribute), false)[0]).category : "Uncategorized").ToList();
                
                var groups = new List<string>();
                foreach (var type in types)
                {
                    var node = (ActionNode)Activator.CreateInstance(type);
                    void Invoke() => CreateNode(type, context);
                    
                    if (node != null && !groups.Contains(node.Category))
                    {
                        groups.Add(node.Category);
                        tree.Add(new SearchTreeGroupEntry(new GUIContent(node.Category)) { level = 2 });
                    }
                    
                    tree.Add(new SearchTreeEntry(new GUIContent($"{node.Name}", node.Description)) { level = 3, userData = (Action)Invoke });
                }
            }
            
            {
                tree.Add(new SearchTreeGroupEntry(new GUIContent("Conditions")) { level = 1 });
                var types = new List<Type>(TypeCache.GetTypesDerivedFrom<ConditionNode>());
                types = types.OrderBy(t => t.GetCustomAttributes(typeof(CategoryAttribute), false).Length > 0 ? ((CategoryAttribute)t.GetCustomAttributes(typeof(CategoryAttribute), false)[0]).category : "Uncategorized").ToList();
                
                var groups = new List<string>();
                foreach (var type in types)
                {
                    var node = (ConditionNode)Activator.CreateInstance(type);
                    void Invoke() => CreateNode(type, context);
                    
                    if (node != null && !groups.Contains(node.Category))
                    {
                        groups.Add(node.Category);
                        tree.Add(new SearchTreeGroupEntry(new GUIContent(node.Category)) { level = 2 });
                    }
                    
                    tree.Add(new SearchTreeEntry(new GUIContent($"{node.Name}", node.Description)) { level = 3, userData = (Action)Invoke });
                }
            }
            
            {
                tree.Add(new SearchTreeGroupEntry(new GUIContent("Composites")) { level = 1 });
                {
                    var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
                    foreach (var type in types)
                    {
                        var node = (CompositeNode)Activator.CreateInstance(type);
                        
                        void Invoke() => CreateNode(type, context);
                        tree.Add(new SearchTreeEntry(new GUIContent($"{node.Name}", node.Description)) { level = 2, userData = (Action)Invoke });
                    }
                }
            }
            
            {
                tree.Add(new SearchTreeGroupEntry(new GUIContent("Decorators")) { level = 1 });
                {
                    var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
                    foreach (var type in types)
                    {
                        var node = (DecoratorNode)Activator.CreateInstance(type);
                        
                        void Invoke() => CreateNode(type, context);
                        tree.Add(new SearchTreeEntry(new GUIContent($"{node.Name}", node.Description)) {level = 2, userData = (Action)Invoke});
                    }
                }
            }

            {
                tree.Add(new SearchTreeGroupEntry(new GUIContent("New Script...")) { level = 1 });

                void CreateActionScript() => CreateScript(_scriptFileAssets[0], context);
                tree.Add(new SearchTreeEntry(new GUIContent($"New Action Script")) { level = 2, userData = (Action)CreateActionScript });

                void CreateCompositeScript() => CreateScript(_scriptFileAssets[1], context);
                tree.Add(new SearchTreeEntry(new GUIContent($"New Composite Script")) { level = 2, userData = (Action)CreateCompositeScript });

                void CreateDecoratorScript() => CreateScript(_scriptFileAssets[2], context);
                tree.Add(new SearchTreeEntry(new GUIContent($"New Decorator Script")) { level = 2, userData = (Action)CreateDecoratorScript });
                
                void CreateConditionScript() => CreateScript(_scriptFileAssets[3], context);
                tree.Add(new SearchTreeEntry(new GUIContent($"New Condition Script")) { level = 2, userData = (Action)CreateConditionScript });
            }
            
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context) 
        {
            Action invoke = (Action)searchTreeEntry.userData;
            invoke();
            return true;
        }

        private void CreateNode(Type type, SearchWindowContext context) 
        {
            BehaviourTreeEditorWindow editorWindow = BehaviourTreeEditorWindow.Instance;
            
            var windowMousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(editorWindow.rootVisualElement.parent, context.screenMousePosition - editorWindow.position.position);
            var graphMousePosition = editorWindow.treeView.contentViewContainer.WorldToLocal(windowMousePosition);
            var nodeOffset = new Vector2(-75, -20);
            var nodePosition = graphMousePosition + nodeOffset;
            
            NodeView createdNode;
            if (_source != null)
            {
                createdNode = _isSourceParent ? _treeView.CreateNode(type, nodePosition, _source) : _treeView.CreateNodeWithChild(type, nodePosition, _source);
            } 
            else 
            {
                createdNode = _treeView.CreateNode(type, nodePosition, null);
            }

            _treeView.SelectNode(createdNode);
        }

        private void CreateScript(EditorUtility.ScriptTemplate scriptTemplate, SearchWindowContext context) 
        {
            BehaviourTreeEditorWindow editorWindow = BehaviourTreeEditorWindow.Instance;

            var windowMousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(editorWindow.rootVisualElement.parent, context.screenMousePosition - editorWindow.position.position);
            var graphMousePosition = editorWindow.treeView.contentViewContainer.WorldToLocal(windowMousePosition);
            var nodeOffset = new Vector2(-75, -20);
            var nodePosition = graphMousePosition + nodeOffset;

            EditorUtility.CreateNewScript(scriptTemplate, _source, _isSourceParent, nodePosition);
        }

        public static void Show(Vector2 mousePosition, NodeView source, bool isSourceParent = false) 
        {
            Vector2 screenPoint = GUIUtility.GUIToScreenPoint(mousePosition);
            CreateNodeWindow searchWindowProvider = CreateInstance<CreateNodeWindow>();
            searchWindowProvider.Initialise(BehaviourTreeEditorWindow.Instance.treeView, source, isSourceParent);
            SearchWindowContext windowContext = new SearchWindowContext(screenPoint, 240, 320);
            SearchWindow.Open(windowContext, searchWindowProvider);
        }
    }
}
