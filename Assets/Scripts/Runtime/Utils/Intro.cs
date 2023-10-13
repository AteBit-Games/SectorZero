using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace Runtime.Utils
{
    public class Intro : MonoBehaviour
    {
        public void Start()
        {
            StartCoroutine(LoadMenuSceneAsync());
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
        }
    }
}