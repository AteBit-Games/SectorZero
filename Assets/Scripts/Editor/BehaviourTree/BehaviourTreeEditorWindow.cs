using System;
using Runtime.BehaviourTree;
using Runtime.BehaviourTree.Monsters;
using Runtime.Utils;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.BehaviourTree 
{
    public class BehaviourTreeEditorWindow : EditorWindow 
    {
        [Serializable]
        public class PendingScriptCreate 
        {
            public bool pendingCreate;
            public string scriptName = "";
            public string sourceGuid = "";
            public bool isSourceParent;
            public Vector2 nodePosition;

            public void Reset() 
            {
                pendingCreate = false;
                scriptName = "";
                sourceGuid = "";
                isSourceParent = false;
                nodePosition = Vector2.zero;
            }
        }

        public class BehaviourTreeEditorAssetModificationProcessor : AssetModificationProcessor 
        {
            private static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions opt) 
            {
                if (HasOpenInstances<BehaviourTreeEditorWindow>()) 
                {
                    BehaviourTreeEditorWindow wnd = GetWindow<BehaviourTreeEditorWindow>();
                    wnd.ClearIfSelected(path);
                }
                return AssetDeleteResult.DidNotDelete;
            }
        }
        public static BehaviourTreeEditorWindow instance;
        public BehaviourTreeProjectSettings settings;
        public VisualTreeAsset behaviourTreeXml;
        public VisualTreeAsset nodeXml;
        public StyleSheet behaviourTreeStyle;
        public TextAsset scriptTemplateActionNode;
        public TextAsset scriptTemplateCompositeNode;
        public TextAsset scriptTemplateDecoratorNode;
        public TextAsset scriptTemplateConditionNode;
        
        public BehaviourTreeView treeView;
        private InspectorView _inspectorView;
        private BlackboardView _blackboardView;
        private OverlayView _overlayView;
        private ToolbarMenu _toolbarMenu;
        private Label _treeNameLabel;
        public NewScriptDialogView newScriptDialog;
        public ToolbarBreadcrumbs breadcrumbs;

        [SerializeField]
        public PendingScriptCreate pendingScriptCreate = new();

        [HideInInspector]
        public Runtime.BehaviourTree.BehaviourTree tree;
        public SerializedBehaviourTree serializer;

        [MenuItem("Editor/Behaviour Tree Editor")]
        public static void OpenWindow()
        {
            BehaviourTreeEditorWindow wnd = GetWindow<BehaviourTreeEditorWindow>();
            wnd.titleContent = new GUIContent("BehaviourTreeEditor");
            wnd.minSize = new Vector2(800, 600);
        }

        private static void OpenWindow(Runtime.BehaviourTree.BehaviourTree tree) 
        {
            BehaviourTreeEditorWindow wnd = GetWindow<BehaviourTreeEditorWindow>();
            wnd.titleContent = new GUIContent("BehaviourTreeEditor");
            wnd.minSize = new Vector2(800, 600);
            wnd.SelectNewTree(tree);
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line) 
        {
            if (Selection.activeObject is Runtime.BehaviourTree.BehaviourTree) 
            {
                OpenWindow(Selection.activeObject as Runtime.BehaviourTree.BehaviourTree);
                return true;
            }
            return false;
        }

        public void CreateGUI()
        {
            instance = this; 
            settings = BehaviourTreeProjectSettings.GetOrCreateSettings();

            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = behaviourTreeXml;
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            var styleSheet = behaviourTreeStyle;
            root.styleSheets.Add(styleSheet);
            
            treeView = root.Q<BehaviourTreeView>();
            _inspectorView = root.Q<InspectorView>();
            _blackboardView = root.Q<BlackboardView>();
            _toolbarMenu = root.Q<ToolbarMenu>();
            _overlayView = root.Q<OverlayView>("select-tree-overlay");
            newScriptDialog = root.Q<NewScriptDialogView>("new-script-overlay");
            breadcrumbs = root.Q<ToolbarBreadcrumbs>("breadcrumbs");
            _treeNameLabel = root.Q<Label>("active-tree");
            
            //select main-container
            var mainContainer = root.Q("main-container");
            mainContainer.ClearClassList();
            mainContainer.AddToClassList("none-selected");

            treeView.styleSheets.Add(behaviourTreeStyle);
            
            _toolbarMenu.RegisterCallback<MouseEnterEvent>(_ => {
                _toolbarMenu.menu.MenuItems().Clear();
                var behaviourTrees = EditorUtility.GetAssetPaths<Runtime.BehaviourTree.BehaviourTree>();
                behaviourTrees.ForEach(path => {
                    var fileName = System.IO.Path.GetFileName(path);
                    _toolbarMenu.menu.AppendAction($"{fileName}", _ => {
                        var behaviourTree = AssetDatabase.LoadAssetAtPath<Runtime.BehaviourTree.BehaviourTree>(path);
                        SelectNewTree(behaviourTree);
                        
                    });
                });
                _toolbarMenu.menu.AppendSeparator();
                _toolbarMenu.menu.AppendAction("New Tree...", _ => OnToolbarNewAsset());
            });
            
            treeView.onNodeSelected -= OnNodeSelectionChanged;
            treeView.onNodeSelected += OnNodeSelectionChanged;

            // Overlay view
            _overlayView.OnTreeSelected -= SelectTree;
            _overlayView.OnTreeSelected += SelectTree;

            // New Script Dialog
            newScriptDialog.style.visibility = Visibility.Hidden;

            if (serializer == null) 
            {
                _overlayView.Show();
            } 
            else 
            {
                SelectTree(serializer.tree);
            }
            
            if (pendingScriptCreate != null && pendingScriptCreate.pendingCreate) {
                CreatePendingScriptNode();
            }
        }

        private void CreatePendingScriptNode()
        {
            NodeView source = treeView.GetNodeByGuid(pendingScriptCreate.sourceGuid) as NodeView;
            var nodeType = Type.GetType($"{pendingScriptCreate.scriptName}, Assembly-CSharp");
            if (nodeType != null)
            {
                NodeView createdNode;
                if (source != null)
                {
                    createdNode = pendingScriptCreate.isSourceParent ? treeView.CreateNode(nodeType, pendingScriptCreate.nodePosition, source) : treeView.CreateNodeWithChild(nodeType, pendingScriptCreate.nodePosition, source);
                } 
                else 
                {
                    createdNode = treeView.CreateNode(nodeType, pendingScriptCreate.nodePosition, null);
                }
                treeView.SelectNode(createdNode);
            }

            pendingScriptCreate.Reset();
        }

        private void OnUndoRedo() 
        {
            if (tree != null) 
            {
                serializer.serializedObject.Update();
                treeView.PopulateView(serializer);
            }
        }

        private void OnEnable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
            Undo.undoRedoPerformed += OnUndoRedo;

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable() 
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj) 
        {
            switch (obj) {
                case PlayModeStateChange.EnteredEditMode:
                    EditorApplication.delayCall += OnSelectionChange;
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    EditorApplication.delayCall += OnSelectionChange;
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    _inspectorView?.Clear();
                    break;
            }
        }

        private void OnSelectionChange() 
        {
            if (Selection.activeGameObject) 
            {
                BehaviourTreeOwner owner = Selection.activeGameObject.GetComponent<BehaviourTreeOwner>();
                if (owner) 
                {
                    SelectNewTree(owner.behaviourTree);
                }
            }
        }

        private void SelectNewTree(Runtime.BehaviourTree.BehaviourTree newTree) 
        {
            ClearBreadcrumbs();
            SelectTree(newTree);
        }

        private void SelectTree(Runtime.BehaviourTree.BehaviourTree newTree) 
        {
            if (treeView == null) return;

            if (!newTree) 
            {
                ClearSelection();
                return;
            }

            if (newTree != tree) ClearSelection();

            tree = newTree;
            serializer = new SerializedBehaviourTree(newTree);
            
            int childCount = breadcrumbs.childCount;
            breadcrumbs.PushItem($"{serializer.tree.name}", () => PopToSubtree(childCount, newTree));
            
            _overlayView?.Hide();
            treeView?.PopulateView(serializer);
            _blackboardView?.Bind(serializer);
            _treeNameLabel.text = newTree.name.SplitCamelCase();
        }

        private void ClearSelection() 
        {
            tree = null;
            serializer = null;
            _inspectorView?.Clear();
            treeView?.ClearView();
            _blackboardView?.ClearView();
            _overlayView?.Show();
        }

        private void ClearIfSelected(string path)
        {
            if (serializer == null) return;
            if (AssetDatabase.GetAssetPath(serializer.tree) == path) 
            {
                EditorApplication.delayCall += () => {
                    SelectTree(null);
                };
            }
        }

        private void OnNodeSelectionChanged(NodeView node)
        {
            _inspectorView.UpdateSelection(serializer, node);
        }

        private void OnInspectorUpdate()
        {
            if (Application.isPlaying)
            {
                treeView?.UpdateNodeStates();
            }
        }

        private void OnToolbarNewAsset() 
        {
            Runtime.BehaviourTree.BehaviourTree newTree = EditorUtility.CreateNewTree("New Behaviour Tree", settings.newTreePath);
            if (newTree) 
            {
                SelectNewTree(newTree);
            }
        }
        
        public void PushSubTreeView(SubTree subtreeNode) 
        {
            if (subtreeNode.treeAsset != null) {
                if (Application.isPlaying) 
                {
                    SelectTree(subtreeNode.treeInstance);
                } 
                else 
                {
                    SelectTree(subtreeNode.treeAsset);
                }
            } 
            else 
            {
                Debug.LogError("Invalid subtree assigned. Assign a a behaviour tree to the tree asset field");
            }
        }
        
        public void PopToSubtree(int depth, Runtime.BehaviourTree.BehaviourTree tree) 
        {
            while (breadcrumbs != null && breadcrumbs.childCount > depth)
            {
                breadcrumbs.PopItem();
            }
            
            if (tree) SelectTree(tree);
        }
        
        public void ClearBreadcrumbs() 
        {
            PopToSubtree(0, null);
        }
    }
}