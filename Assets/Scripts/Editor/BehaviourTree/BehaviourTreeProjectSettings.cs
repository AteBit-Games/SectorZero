using UnityEditor;
using UnityEngine;

namespace Editor.BehaviourTree
{
    public class BehaviourTreeProjectSettings : ScriptableObject 
    {

        [Tooltip("Folder where new behaviour trees will be created.")]
        public string newTreePath = "Assets/ScriptableObjects/AI/";

        [Tooltip("Folder where new node scripts will be created.")]
        public string newNodePath = "Assets/Scripts/Runtime/BehaviourTree/Nodes/";

        [Tooltip("Script template to use when creating action nodes")]
        public TextAsset scriptTemplateActionNode;

        [Tooltip("Script template to use when creating composite nodes")]
        public TextAsset scriptTemplateCompositeNode;

        [Tooltip("Script template to use when creating decorator nodes")]
        public TextAsset scriptTemplateDecoratorNode;
        
        [Tooltip("Script template to use when creating condition nodes")]
        public TextAsset scriptTemplateConditionNode;

        private static BehaviourTreeProjectSettings FindSettings()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(BehaviourTreeProjectSettings)}");
            if (guids.Length > 1) Debug.LogWarning($"Found multiple settings files, using the first.");

            switch (guids.Length) 
            {
                case 0:
                    return null;
                default:
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    return AssetDatabase.LoadAssetAtPath<BehaviourTreeProjectSettings>(path);
            }
        }

        internal static BehaviourTreeProjectSettings GetOrCreateSettings() 
        {
            var settings = FindSettings();
            
            if (settings == null) 
            {
                settings =CreateInstance<BehaviourTreeProjectSettings>();
                AssetDatabase.CreateAsset(settings, "Assets/ScriptableObjects/AI/BehaviourTreeProjectSettings.asset");
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings() 
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
}
