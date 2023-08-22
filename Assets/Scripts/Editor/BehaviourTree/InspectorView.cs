using System.Collections.Generic;
using Runtime.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.BehaviourTree 
{
    public class InspectorView : VisualElement 
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }
        
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

            //list element
            var list = new ScrollView
            {
                style =
                {
                    maxHeight = 400,
                    overflow = Overflow.Hidden
                }
            };
            
            foreach (var child in GetChildren(nodeProperty))
            {
                if (child.name == "m_Script") continue;
                var propertyField = new PropertyField(child);
                propertyField.Bind(child.serializedObject);
                list.Add(propertyField);
            }
            
            Add(list);
        }

        private static IEnumerable<SerializedProperty> GetChildren(SerializedProperty property)
        {
            property = property.Copy();
            var nextElement = property.Copy();
            bool hasNextElement = nextElement.NextVisible(false);
            if (!hasNextElement)
            {
                nextElement = null;
            }

            property.NextVisible(true);
            while (true)
            {
                if ((SerializedProperty.EqualContents(property, nextElement)))
                {
                    yield break;
                }

                yield return property;

                bool hasNext = property.NextVisible(false);
                if (!hasNext)
                {
                    break;
                }
            }
        }
    }
}