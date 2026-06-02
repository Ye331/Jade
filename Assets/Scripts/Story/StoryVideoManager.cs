using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class StoryVideoManager : MonoBehaviour
{
    [Header("视频播放器")]
    public VideoPlayer videoPlayer;

    [Header("视频显示画面")]
    public RawImage videoScreen;

    [Header("跳过按钮")]
    public Button skipButton;

    [Header("视频结束后进入的场景")]
    public string nextSceneName = "";

    [Header("测试模式：勾选时不跳转场景")]
    public bool testMode = true;

    [Header("剧情存档标记")]
    public string storyKey = "HasPlayedGlobalStory";

    private bool hasFinished = false;

    private void Start()
    {
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipVideo);
        }
        else
        {
            Debug.LogWarning("StoryVideoManager：SkipButton 没有绑定。");
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.Play();
        }
        else
        {
            Debug.LogError("StoryVideoManager：VideoPlayer 没有绑定。");
        }
    }

    private void OnDestroy()
    {
        if (skipButton != null)
        {
            skipButton.onClick.RemoveListener(SkipVideo);
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        FinishStory();
    }

    private void SkipVideo()
    {
        Debug.Log("点击了 Skip Intro。");
        FinishStory();
    }

    private void FinishStory()
    {
        if (hasFinished)
        {
            return;
        }

        hasFinished = true;

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }

        if (videoScreen != null)
        {
            videoScreen.gameObject.SetActive(false);
        }

        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(false);
        }

        PlayerPrefs.SetInt(storyKey, 1);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            PlayerPrefs.SetString("LastSceneName", nextSceneName);
        }

        PlayerPrefs.Save();

        if (testMode)
        {
            Debug.Log("剧情视频结束：测试模式下不跳转场景。");
            return;
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("StoryVideoManager：没有填写 nextSceneName。");
        }
    }
}