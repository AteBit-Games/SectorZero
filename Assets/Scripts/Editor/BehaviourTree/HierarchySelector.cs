using UnityEngine.UIElements;

namespace Editor.BehaviourTree {
    public class HierarchySelector : MouseManipulator 
    {

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
            if (!CanStopManipulation(evt)) return;
            if (target is not BehaviourTreeView graphView) return;

            if (evt.target is not NodeView clickedElement) 
            {
                var ve = evt.target as VisualElement;
                clickedElement = ve.GetFirstAncestorOfType<NodeView>();
                if (clickedElement == null) return;
            }

            if (evt.ctrlKey) SelectChildren(evt, graphView, clickedElement);
        }

        static void SelectChildren(MouseDownEvent evt, BehaviourTreeView graphView, NodeView clickedElement) 
        {
            Runtime.BehaviourTree.BehaviourTree.Traverse(clickedElement.node, node => {
                var view = graphView.FindNodeView(node);
                graphView.AddToSelection(view);
            });
        }
    }
}