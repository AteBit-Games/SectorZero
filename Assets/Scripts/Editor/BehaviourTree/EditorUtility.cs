using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor.BehaviourTree
{
    public static class EditorUtility
    {
        public struct ScriptTemplate {
            public TextAsset templateFile;
            public string defaultFileName;
            public string subFolder;
        }

        [System.Serializable]
        public class PackageManifest {
            public string name;
            public string version;
        }

        public static Runtime.BehaviourTree.BehaviourTree CreateNewTree(string assetName, string folder) 
        {
            string path = System.IO.Path.Join(folder, $"{assetName}.asset");
            if (System.IO.File.Exists(path)) {
                Debug.LogError($"Failed to create behaviour tree asset: Path already exists:{assetName}");
                return null;
            }
            
            Runtime.BehaviourTree.BehaviourTree tree = ScriptableObject.CreateInstance<Runtime.BehaviourTree.BehaviourTree>();
            tree.name = assetName;
            AssetDatabase.CreateAsset(tree, path);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(tree);
            return tree;
        }

        public static void CreateNewScript(ScriptTemplate scriptTemplate, NodeView source, bool isSourceParent, Vector2 position) 
        {
            BehaviourTreeEditorWindow.instance.newScriptDialog.CreateScript(scriptTemplate, source, isSourceParent, position);
        }


        public static List<T> LoadAssets<T>() where T : Object 
        {
            string[] assetIds = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            return assetIds.Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<T>).ToList();
        }

        public static List<string> GetAssetPaths<T>() where T : Object 
        {
            string[] assetIds = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            return assetIds.Select(AssetDatabase.GUIDToAssetPath).ToList();
        }

        public static float RoundTo(float value, int nearestInteger)
        {
            return (Mathf.FloorToInt(value / nearestInteger)) * nearestInteger;
        }

    }
}
