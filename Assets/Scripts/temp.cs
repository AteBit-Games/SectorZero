
using Runtime.Managers;
using Runtime.Player;
using Runtime.Utils;
using UnityEngine;

[DefaultExecutionOrder(999)]
public class temp : MonoBehaviour
{
    private PlayerController _player;
    private bool _done;

    private void Start()
    {
        _player = FindObjectOfType<PlayerController>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !_done)
        {
            GameManager.Instance.SaveSystem.SaveGame();
            // _player.Die(DeathType.Locker);
            _done = true;
        }
    }
    
    public void DebugMessage(string message)
    {
        Debug.Log(message);
    }
}
