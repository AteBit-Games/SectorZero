using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public static class CustomInspectorUtils
    {
        public static void OpenScriptForEditor(object target)
        {
            var scriptName = target.GetType().Name;
            var assetGuids = AssetDatabase.FindAssets($"t:TextAsset {scriptName}");
                    
            foreach (var t in assetGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(t);
                var filename = System.IO.Path.GetFileName(path);
                if (filename == $"{scriptName}.cs") {
                    var script = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                    AssetDatabase.OpenAsset(script);
                    break;
                }
            }
        }
    }
}