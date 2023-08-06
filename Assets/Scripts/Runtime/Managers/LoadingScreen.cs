using System.Collections;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.Managers
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Sprite[] iconStates;
        [SerializeField] private float iconSpeed = 0.5f;
        [HideInInspector] public bool isOpen;

        private UIDocument _uiDocument;

        private VisualElement _loadContainer;
        private VisualElement _loadingIcon;
        private VisualElement _continueText;
        
        private int _loadingIndex;
        private Coroutine _loadingIconCoroutine;
        
        private void Start()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            
            _loadContainer = rootVisualElement.Q<VisualElement>("load-overlay");
            _continueText = rootVisualElement.Q<VisualElement>("loading-label");
            _loadingIcon = rootVisualElement.Q<VisualElement>("loading-icon");
        }
        
        public void ShowLoading()
        {
            if(_loadingIconCoroutine != null) StopCoroutine(_loadingIconCoroutine);
            _loadingIconCoroutine = StartCoroutine(LoadingIcon());
            UI_Utils.ShowUIElement(_loadContainer);
            _continueText.visible = false;
            isOpen = true;
        }
        
        public void HideLoading()
        {
            if(_loadingIconCoroutine != null) StopCoroutine(_loadingIconCoroutine);
            UI_Utils.HideUIElement(_loadContainer);
            isOpen = false;
            GameManager.Instance.SoundSystem.ResumeAll();
        }

        public void ShowContinue()
        {
            _continueText.visible = true;
        }

        private IEnumerator LoadingIcon()
        {
            _loadingIcon.style.backgroundImage = new StyleBackground(iconStates[_loadingIndex]);
            _loadingIndex = (_loadingIndex + 1) % iconStates.Length;
            yield return new WaitForSecondsRealtime(iconSpeed);
            _loadingIconCoroutine = StartCoroutine(LoadingIcon());
        }
    }
}
