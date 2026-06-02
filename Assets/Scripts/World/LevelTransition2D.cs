using Jade.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jade.World
{
    [RequireComponent(typeof(Collider2D))]
    public class LevelTransition2D : MonoBehaviour
    {
        [SerializeField] private string targetSceneName;
        [SerializeField] private string loadingSceneName;
        [SerializeField] private float loadingMinimumSeconds = 0.75f;

        private bool triggered;

        private void Reset()
        {
            Collider2D trigger = GetComponent<Collider2D>();
            trigger.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggered || other.GetComponent<PlayerMotor2D>() == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(targetSceneName))
            {
                Debug.LogWarning("Level transition has no target scene: " + gameObject.name);
                return;
            }

            triggered = true;
            if (!string.IsNullOrWhiteSpace(loadingSceneName))
            {
                LoadingSceneBridge.TargetSceneName = targetSceneName;
                LoadingSceneBridge.MinimumSeconds = loadingMinimumSeconds;
                SceneManager.LoadScene(loadingSceneName);
                return;
            }

            SceneManager.LoadScene(targetSceneName);
        }
    }
}
