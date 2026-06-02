using Jade.CameraTools;
using Jade.Player;
using UnityEngine;

namespace Jade.World
{
    public class SceneEntrySpawnPoint2D : MonoBehaviour
    {
        private const string DefaultPlayerPrefabResourcePath = "Prefabs/Player/JadeQilinPlayer";

        [SerializeField] private bool applyOnStart = true;
        [SerializeField] private Vector2 entryVelocity = new Vector2(0f, -8f);
        [SerializeField] private bool setRespawnPoint = true;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private string playerPrefabResourcePath = DefaultPlayerPrefabResourcePath;

        private void Start()
        {
            if (!applyOnStart)
            {
                return;
            }

            PlayerMotor2D player = FindOrCreatePlayer();
            if (player == null)
            {
                Debug.LogWarning("Scene entry spawn point could not find or create PlayerMotor2D.");
                return;
            }

            if (setRespawnPoint)
            {
                player.SetSpawnPoint(transform.position);
            }

            player.TeleportTo(transform.position, entryVelocity);
            ConfigureCamera(player.transform);
        }

        private PlayerMotor2D FindOrCreatePlayer()
        {
            PlayerMotor2D existingPlayer = FindAnyObjectByType<PlayerMotor2D>();
            if (existingPlayer != null)
            {
                return existingPlayer;
            }

            GameObject prefab = playerPrefab != null ? playerPrefab : LoadPlayerPrefab();
            if (prefab == null)
            {
                Debug.LogWarning("Scene entry spawn point could not load a player prefab.");
                return null;
            }

            GameObject playerInstance = Instantiate(prefab, transform.position, Quaternion.identity);
            playerInstance.name = prefab.name;

            PlayerMotor2D player = playerInstance.GetComponent<PlayerMotor2D>();
            if (player == null)
            {
                Debug.LogWarning("Scene entry spawn point created a prefab without PlayerMotor2D.");
            }

            return player;
        }

        private GameObject LoadPlayerPrefab()
        {
            if (string.IsNullOrWhiteSpace(playerPrefabResourcePath))
            {
                return null;
            }

            return Resources.Load<GameObject>(playerPrefabResourcePath);
        }

        private static void ConfigureCamera(Transform playerTransform)
        {
            SimpleCameraFollow2D cameraFollow = FindAnyObjectByType<SimpleCameraFollow2D>();
            if (cameraFollow != null)
            {
                cameraFollow.Configure(playerTransform);
            }
        }
    }
}
