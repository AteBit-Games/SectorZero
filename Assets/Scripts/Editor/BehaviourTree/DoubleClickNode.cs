using Runtime.BehaviourTree.Actions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.BehaviourTree 
{
    public class  DoubleClickNode : MouseManipulator 
    {
        private double time;
        private const double doubleClickDuration = 0.3;

        public DoubleClickNode() 
        {
            time = EditorApplication.timeSinceStartup;
        }

        protected override void RegisterCallbacksOnTarget() 
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        protected override void UnregisterCallbacksFromTarget() 
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnMouseDown(MouseDownEvent evt) 
        {
            if (!CanStopManipulation(evt))
                return;

            if (evt.target is not NodeView clickedElement) 
            {
                var ve = evt.target as VisualElement;
                clickedElement = ve.GetFirstAncestorOfType<NodeView>();
                if (clickedElement == null) return;
            }

            double duration = EditorApplication.timeSinceStartup - time;
            if (duration < doubleClickDuration) OnDoubleClick(evt, clickedElement);

            time = EditorApplication.timeSinceStartup;
        }

        private static void OpenScriptForNode(MouseDownEvent evt, NodeView clickedElement) 
        {
            var nodeName = clickedElement.node.GetType().Name;
            var assetGuids = AssetDatabase.FindAssets($"t:TextAsset {nodeName}");
            
            foreach (var t in assetGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(t);
                var filename = System.IO.Path.GetFileName(path);
                if (filename == $"{nodeName}.cs") {
                    var script = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                    AssetDatabase.OpenAsset(script);
                    break;
                }
            }
            
            BehaviourTreeEditorWindow.Instance.treeView.RemoveFromSelection(clickedElement);
        }
        
        private static void OnDoubleClick(MouseDownEvent evt, NodeView clickedElement) 
        {
            OpenScriptForNode(evt, clickedElement);
        }
    }
}