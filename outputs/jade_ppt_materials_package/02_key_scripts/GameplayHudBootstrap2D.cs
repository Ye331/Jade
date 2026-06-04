using System.Collections;
using Jade.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jade.UI
{
    public class GameplayHudBootstrap2D : MonoBehaviour
    {
        private Coroutine attachRoutine;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateBootstrap()
        {
            if (FindObjectOfType<GameplayHudBootstrap2D>() != null)
            {
                return;
            }

            GameObject bootstrap = new GameObject("GameplayHudBootstrap2D");
            DontDestroyOnLoad(bootstrap);
            bootstrap.AddComponent<GameplayHudBootstrap2D>();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
            StartAttachRoutine();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StartAttachRoutine();
        }

        private void StartAttachRoutine()
        {
            if (attachRoutine != null)
            {
                StopCoroutine(attachRoutine);
            }

            attachRoutine = StartCoroutine(AttachWhenPlayerExists());
        }

        private IEnumerator AttachWhenPlayerExists()
        {
            float endTime = Time.realtimeSinceStartup + 3f;
            while (Time.realtimeSinceStartup < endTime)
            {
                PlayerAbilityInventory2D abilities = FindObjectOfType<PlayerAbilityInventory2D>();
                PlayerHealth2D health = abilities != null ? abilities.GetComponent<PlayerHealth2D>() : FindObjectOfType<PlayerHealth2D>();
                if (abilities != null && health != null)
                {
                    GameplayHud2D hud = FindObjectOfType<GameplayHud2D>();
                    if (hud == null)
                    {
                        GameObject hudObject = new GameObject("GameplayHud2D");
                        hud = hudObject.AddComponent<GameplayHud2D>();
                    }

                    hud.Configure(abilities, health);
                    attachRoutine = null;
                    yield break;
                }

                yield return null;
            }

            attachRoutine = null;
        }
    }
}
