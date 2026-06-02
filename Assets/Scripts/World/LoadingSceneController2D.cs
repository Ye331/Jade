using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jade.World
{
    public class LoadingSceneController2D : MonoBehaviour
    {
        [SerializeField] private string fallbackTargetSceneName;
        [SerializeField] private float fallbackMinimumSeconds = 0.75f;

        private IEnumerator Start()
        {
            string target = !string.IsNullOrWhiteSpace(LoadingSceneBridge.TargetSceneName)
                ? LoadingSceneBridge.TargetSceneName
                : fallbackTargetSceneName;

            if (string.IsNullOrWhiteSpace(target))
            {
                Debug.LogWarning("Loading scene has no target scene.");
                yield break;
            }

            float minimumSeconds = LoadingSceneBridge.MinimumSeconds > 0f
                ? LoadingSceneBridge.MinimumSeconds
                : fallbackMinimumSeconds;

            AsyncOperation operation = SceneManager.LoadSceneAsync(target);
            operation.allowSceneActivation = false;

            float timer = 0f;
            while (timer < minimumSeconds || operation.progress < 0.9f)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            LoadingSceneBridge.TargetSceneName = null;
            operation.allowSceneActivation = true;
        }
    }
}
