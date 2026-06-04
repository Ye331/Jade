using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("总剧情场景")]
    public string globalStorySceneName = "StoryIntro";

    [Header("默认关卡场景")]
    public string defaultLevelSceneName = "ShanhaiGate";

    [Header("关卡选择场景")]
    public string levelSelectSceneName = "LevelSelect";

    [Header("设置面板")]
    public GameObject settingsPanel;

    [Header("总剧情播放标记")]
    public string globalStoryKey = "HasPlayedGlobalStory";

    [Header("转场控制器")]
    public SceneTransition sceneTransition;

    public void OnStartGameClicked()
    {
        Debug.Log("点击了 Start Game");

        bool hasPlayedGlobalStory = PlayerPrefs.GetInt(globalStoryKey, 0) == 1;

        string targetSceneName;

        if (!hasPlayedGlobalStory)
        {
            targetSceneName = globalStorySceneName;
            Debug.Log("第一次游玩，转场到总剧情：" + targetSceneName);
        }
        else
        {
            string lastSceneName = PlayerPrefs.GetString("LastSceneName", "");

            if (!string.IsNullOrEmpty(lastSceneName))
            {
                targetSceneName = lastSceneName;
                Debug.Log("已播放剧情，转场到上一次场景：" + targetSceneName);
            }
            else
            {
                targetSceneName = defaultLevelSceneName;
                Debug.Log("没有上一次场景，转场到默认关卡：" + targetSceneName);
            }
        }

        LoadSceneWithTransition(targetSceneName);
    }

    public void OnLevelSelectClicked()
    {
        LoadSceneWithTransition(levelSelectSceneName);
    }

    public void OnSettingsClicked()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void OnCloseSettingsClicked()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void OnQuitGameClicked()
    {
        Debug.Log("退出游戏");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ResetProgressForTest()
    {
        PlayerPrefs.DeleteKey(globalStoryKey);
        PlayerPrefs.DeleteKey("HasPlayedBambooStory");
        PlayerPrefs.DeleteKey("LastSceneName");
        PlayerPrefs.DeleteKey("LastPlayerX");
        PlayerPrefs.DeleteKey("LastPlayerY");
        PlayerPrefs.DeleteKey("LastPlayerZ");
        PlayerPrefs.Save();

        Debug.Log("测试进度已清除");
    }

    private void LoadSceneWithTransition(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("MainMenuManager：目标场景名为空。");
            return;
        }

        if (sceneTransition != null)
        {
            sceneTransition.TransitionToScene(sceneName);
        }
        else
        {
            Debug.LogWarning("MainMenuManager：没有绑定 SceneTransition，直接跳转。");
            SceneManager.LoadScene(sceneName);
        }
    }
}