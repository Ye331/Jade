using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jade.World
{
    public class LoadingSceneController2D : MonoBehaviour
    {
        [SerializeField] private string fallbackTargetSceneName;
        [SerializeField] private float fallbackMinimumSeconds = 0.75f;
        [SerializeField] private SpriteRenderer progressRenderer;
        [SerializeField] private Sprite[] progressFrames;

        private IEnumerator Start()
        {
            UpdateProgressFrame(0f);

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
                float loadProgress = Mathf.Clamp01(operation.progress / 0.9f);
                float timeProgress = minimumSeconds > 0f ? Mathf.Clamp01(timer / minimumSeconds) : 1f;
                UpdateProgressFrame(Mathf.Min(loadProgress, timeProgress));
                yield return null;
            }

            UpdateProgressFrame(1f);
            LoadingSceneBridge.TargetSceneName = null;
            operation.allowSceneActivation = true;
        }

        private void UpdateProgressFrame(float progress)
        {
            if (progressRenderer == null || progressFrames == null || progressFrames.Length == 0)
            {
                return;
            }

            int frameIndex = Mathf.Clamp(
                Mathf.FloorToInt(Mathf.Clamp01(progress) * progressFrames.Length),
                0,
                progressFrames.Length - 1);

            progressRenderer.sprite = progressFrames[frameIndex];
        }
    }
}
