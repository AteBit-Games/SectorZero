using Runtime.Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Editor.Managers
{
     [CustomEditor(typeof(LightManager))]
     public class CustomListEditor : UnityEditor.Editor
     {
          private LightManager _lightManager;
          private SerializedObject _serializedLightManager;
          private SerializedProperty _sectors;
 

          private void OnEnable()
          {
               _lightManager = (LightManager)target;
               _serializedLightManager = new SerializedObject(_lightManager);
               _sectors = _serializedLightManager.FindProperty("lightSectors.list");
          }
          
          public override void OnInspectorGUI() 
          {
               for (var i = 0; i < _sectors.arraySize; i++)
               {
                    GUILayout.BeginVertical("GroupBox");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("LIGHT GROUP " + i);
                    GUILayout.Space(4);
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                         _sectors.DeleteArrayElementAtIndex(i);
                         break;
                    }
                    GUILayout.EndHorizontal();
                    
                    GUILayout.BeginHorizontal();
                    var sector = _sectors.GetArrayElementAtIndex(i).FindPropertyRelative("lights");
                    EditorGUILayout.PropertyField(sector, true, GUILayout.MinWidth(0));
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                    GUILayout.Space(3);
               }

               if (GUILayout.Button("Add Group"))
               {
                    _sectors.InsertArrayElementAtIndex(_sectors.arraySize);
               }
               GUILayout.Space(3);
               if (GUILayout.Button("Clear"))
               {
                    _sectors.ClearArray();
               }
               
               _serializedLightManager.ApplyModifiedProperties();
          }
     }
}