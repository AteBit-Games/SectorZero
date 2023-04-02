using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;

public class Temp : MonoBehaviour
{
    [SerializeField] private AudioMixGroup audioMixGroup;
    
    private void Start()
    {
        Debug.Log(audioMixGroup.Cues.Count);
    }
}
