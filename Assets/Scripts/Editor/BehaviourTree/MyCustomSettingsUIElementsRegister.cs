using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.BehaviourTree
{
    internal static class MyCustomSettingsUIElementsRegister 
    {
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            
            var provider = new SettingsProvider("Project/BehaviourTreeProjectSettings", SettingsScope.Project)
            {
                label = "Behaviour Tree",
                activateHandler = (searchContext, rootElement) => {
                    var settings = BehaviourTreeProjectSettings.GetSerializedSettings();
                    
                    var title = new Label() {
                        text = "Behaviour Tree Settings"
                    };
                    title.AddToClassList("title");
                    rootElement.Add(title);

                    var properties = new VisualElement() 
                    {
                        style = {
                            flexDirection = FlexDirection.Column
                        }
                    };
                    properties.AddToClassList("property-list");
                    rootElement.Add(properties);

                    properties.Add(new InspectorElement(settings));
                    rootElement.Bind(settings);
                },
            };

            return provider;
        }
    }
}