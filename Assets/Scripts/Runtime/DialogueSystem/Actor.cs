using UnityEngine;

[CreateAssetMenu(fileName = "New Actor", menuName = "Dialogue System/Actor")]
public class Actor : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _sprite;

    public string Name => _name;
    public Sprite Sprite => _sprite;
}