using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("淡入淡出遮罩")]
    public Image fadeImage;

    [Header("淡出时间")]
    public float fadeOutDuration = 0.6f;

    [Header("淡入时间")]
    public float fadeInDuration = 0.4f;

    [Header("进入场景时是否淡入")]
    public bool fadeInOnStart = true;

    private bool isTransitioning = false;

    private void Start()
    {
        if (fadeImage == null)
        {
            Debug.LogWarning("SceneTransition：没有绑定 Fade Image。");
            return;
        }

        if (fadeInOnStart)
        {
            StartCoroutine(FadeIn());
        }
        else
        {
            SetAlpha(0f);
        }
    }

    public void TransitionToScene(string sceneName)
    {
        if (isTransitioning)
        {
            return;
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("SceneTransition：目标场景名为空，无法跳转。");
            return;
        }

        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeIn()
    {
        isTransitioning = true;

        float timer = 0f;

        while (timer < fadeInDuration)
        {
            timer += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeInDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);
        isTransitioning = false;
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        isTransitioning = true;

        float timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeOutDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(1f);
        SceneManager.LoadScene(sceneName);
    }

    private void SetAlpha(float alpha)
    {
        if (fadeImage == null)
        {
            return;
        }

        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}