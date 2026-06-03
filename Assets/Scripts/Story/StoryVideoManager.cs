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

    [Header("转场控制器")]
    public SceneTransition sceneTransition; 

    private bool hasFinished = false;

    private void Start()
    {
        // 判断是否第一次播放剧情
        bool hasPlayed = PlayerPrefs.GetInt(storyKey, 0) == 1;

        string targetScene = nextSceneName;

        if (hasPlayed)
        {
            // 已播放过，跳到上次场景或默认场景
            targetScene = PlayerPrefs.GetString("LastSceneName", nextSceneName);
            LoadSceneWithTransition(targetScene);
            return;
        }

        // 第一次播放剧情，绑定按钮和播放视频
        if (skipButton != null)
            skipButton.onClick.AddListener(SkipVideo);
        else
            Debug.LogWarning("StoryVideoManager：SkipButton 没有绑定。");

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
            skipButton.onClick.RemoveListener(SkipVideo);

        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
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
            return;

        hasFinished = true;

        if (videoPlayer != null)
            videoPlayer.Stop();

        if (videoScreen != null)
            videoScreen.gameObject.SetActive(false);

        if (skipButton != null)
            skipButton.gameObject.SetActive(false);

        PlayerPrefs.SetInt(storyKey, 1);

        if (!string.IsNullOrEmpty(nextSceneName))
            PlayerPrefs.SetString("LastSceneName", nextSceneName);

        PlayerPrefs.Save();

        if (testMode)
        {
            Debug.Log("剧情视频结束：测试模式下不跳转场景。");
            return;
        }

        // 使用统一转场跳转场景
        LoadSceneWithTransition(nextSceneName);
    }

    private void LoadSceneWithTransition(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("StoryVideoManager：目标场景名为空，直接返回。");
            return;
        }

        if (sceneTransition != null)
        {
            sceneTransition.TransitionToScene(sceneName);
        }
        else
        {
            Debug.LogWarning("StoryVideoManager：没有绑定 SceneTransition，直接跳转。");
            SceneManager.LoadScene(sceneName);
        }
    }
}