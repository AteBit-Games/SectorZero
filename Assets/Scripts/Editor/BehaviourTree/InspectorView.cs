using Runtime.Utils;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.BehaviourTree 
{
    public class InspectorView : VisualElement 
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        public InspectorView() { }

        internal void UpdateSelection(SerializedBehaviourTree serializer, NodeView nodeView) 
        {
            Clear();

            if (nodeView == null) return;

            var nodeProperty = serializer.FindNode(serializer.Nodes, nodeView.node);
            if (nodeProperty == null) return;
            
            Label headerLabel = parent.Q<Label>("inspector-header");
            headerLabel.text = nodeView.node.Name.SplitCamelCase().ToUpper();
            
            Label descriptionLabel = parent.Q<Label>("inspector-information");
            descriptionLabel.text = nodeView.node.Description;
            
            nodeProperty.isExpanded = true;
            
            PropertyField field = new PropertyField
            {
                label = nodeProperty.managedReferenceValue.GetType().ToString()
            };
            field.BindProperty(nodeProperty);
            Add(field);
        }
    }
}