using UnityEditor;

namespace Editor
{
    public class RoomNodeGraphEditor : EditorWindow
    {
       [MenuItem("Node Graph Editor", menuItem = "Window/Dungeon Editor/Node Graph Editor")]
       private static void OpenWindow()
       {
           GetWindow<RoomNodeGraphEditor>("Dungeon Editor");
           
       }
    }
}

