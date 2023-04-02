// Copyright (c) 2023 AteBit Games

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Runtime.SoundSystem.Propagation
{
    [CustomEditor(typeof(AudioPropagationNodeGroup))]
    public class AudioPropagationEditor : Editor
    {

        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            AudioPropagationNodeGroup myScript = (AudioPropagationNodeGroup) target;
            
            if (GUILayout.Button("Add Node"))
            {
                myScript.AddNode( Vector3.zero);
            }
            
            if (GUILayout.Button("Remove All Nodes"))
            {
                myScript.DeleteAllNodes();
            }
        }
        
        // private void OnSceneGUI()
        // {
        //     Event e = Event.current;
        //     if (e.type == EventType.KeyDown && e.keyCode == KeyCode.D)
        //     {
        //         AudioPropagationNodeGroup myScript = (AudioPropagationNodeGroup) target;
        //         myScript.AddNode();
        //     }
        // }
    }
}
