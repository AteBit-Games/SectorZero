using System.Collections.Generic;
using UnityEngine;

namespace Runtime.DialogueSystem
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] public List<string> lines;
        [SerializeField] public Actor actor;
    }
}
