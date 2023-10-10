using System.Collections;
using Tweens;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Runtime.Utils
{
    public class Intro : MonoBehaviour
    {
        private bool _splashScreenDone;
        
        private UIDocument _uiDocument;
        private VisualElement _warning;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            
            _warning = rootVisualElement.Q<VisualElement>("Warning");
            _warning.style.opacity = 0;
        }

        public void Start()
        {
            StartCoroutine(LoadMenuSceneAsync());
            //StartCoroutine(ShowSplashScreens());
        }

        private IEnumerator ShowSplashScreens()
        {
            yield return new WaitForSeconds(0.3f);
            
            var warningTween = new FloatTween
            {
                to = 1f,
                from = 0f,
                duration = 0.6f,
                useUnscaledTime = true,
                onUpdate = (_, value) =>
                {
                    _warning.style.opacity = value;
                }
            };
            gameObject.AddTween(warningTween);
            
            yield return new WaitForSeconds(3.5f);
            
            var warningTween2 = new FloatTween
            {
                to = 0f,
                from = 1f,
                duration = 0.6f,
                useUnscaledTime = true,
                onUpdate = (_, value) =>
                {
                    _warning.style.opacity = value;
                }
            };
            
            gameObject.AddTween(warningTween2);
            yield return new WaitForSeconds(1f);
            _splashScreenDone = true;
        }

        private IEnumerator LoadMenuSceneAsync()
        {
            var asyncOperation = Addressables.LoadSceneAsync("MainMenu", LoadSceneMode.Single, false);
            
            while (!asyncOperation.IsDone)
            {
                yield return asyncOperation;
            }
            
            if (asyncOperation.Status == AsyncOperationStatus.Succeeded) 
                yield return asyncOperation.Result.ActivateAsync();
            
            //
            // while(asyncOperation.Status == AsyncOperationStatus.Succeeded)
            // {
            //     if (_splashScreenDone) yield return asyncOperation.Result.ActivateAsync();
            //     yield return new WaitForSeconds(0.05f);
            // }
        }
    }
}