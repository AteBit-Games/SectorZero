
using Runtime.Managers;
using UnityEngine;


namespace Runtime.Utils
{
    public class FadeIn : MonoBehaviour
    {
        private CanvasGroup _canvas;
        private static readonly int Fade = Animator.StringToHash("fade-out");

        private void Awake()
        {
            _canvas = GetComponent<CanvasGroup>();  
        }

        private void Start()
        {
            if (GameManager.Instance.TestMode)
            {
                gameObject.SetActive(false);
            }
            else
            { 
                _canvas.alpha = 1;
                _canvas.GetComponent<Animator>().SetTrigger(Fade);
            }
        }
    }
}