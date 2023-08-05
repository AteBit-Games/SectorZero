
using Runtime.Managers;
using Runtime.Player;
using Runtime.Utils;
using UnityEngine;

[DefaultExecutionOrder(999)]
public class temp : MonoBehaviour
{
    private bool _done;
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !_done)
        {
            GameManager.Instance.EndGame();
            _done = true;
        }
    }
    
}
