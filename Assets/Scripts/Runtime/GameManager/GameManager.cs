using Runtime.Input;
using UnityEngine;

namespace Runtime.GameManager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;

        private bool _isPaused;

        private void Start()
        {
            inputReader.PauseEvent += HandlePause;
            inputReader.ResumeEvent += HandleResume;
        }
        
        private void HandlePause()
        {
            _isPaused = true;
            Time.timeScale = 0;
        }
        
        private void HandleResume()
        {
            _isPaused = false;
            Time.timeScale = 1;
        }
    }
}