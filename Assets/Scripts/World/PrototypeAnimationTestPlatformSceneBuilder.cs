using Jade.CameraTools;
using Jade.Player;
using UnityEngine;

namespace Jade.World
{
    public class PrototypeAnimationTestPlatformSceneBuilder : MonoBehaviour
    {
        [SerializeField] private PlayerMovementSettings movementSettings;
        [SerializeField] private Camera sceneCamera;

        private const string PlayerPrefabResourcePath = "Prefabs/Player/JadeSpiritPlayer";
        private const string LegacyPlayerPrefabResourcePath = "Prefabs/Player/JadeQilinPlayer";
        private static Sprite squareSprite;

        private void Awake()
        {
            BuildScene();
        }

        private void BuildScene()
        {
            Sprite sprite = GetSquareSprite();
            GameObject player = CreatePlayer();
            CreateCamera(player.transform);
            CreateTestPlatforms(sprite);
            CreateRespawnZone();
        }

        private GameObject CreatePlayer()
        {
            Vector3 spawn = new Vector3(-18f, -1.7f, 0f);
            GameObject prefab = Resources.Load<GameObject>(PlayerPrefabResourcePath);
            if (prefab == null)
            {
                prefab = Resources.Load<GameObject>(LegacyPlayerPrefabResourcePath);
            }

            if (prefab != null)
            {
                GameObject playerInstance = Instantiate(prefab, spawn, Quaternion.identity);
                playerInstance.name = "Player_AnimationTest";

                PlayerMotor2D motor = playerInstance.GetComponent<PlayerMotor2D>();
                if (motor != null)
                {
                    if (movementSettings != null)
                    {
                        motor.Configure(movementSettings);
                    }

                    motor.SetSpawnPoint(spawn);
                }

                return playerInstance;
            }

            GameObject player = new GameObject("Player_AnimationTest_Fallback");
            player.transform.position = spawn;

            Rigidbody2D body = player.AddComponent<Rigidbody2D>();
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;

            BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.68f, 1.25f);

            player.AddComponent<PlayerInputReader>();
            PlayerMotor2D fallbackMotor = player.AddComponent<PlayerMotor2D>();
            if (movementSettings != null)
            {
                fallbackMotor.Configure(movementSettings);
            }

            fallbackMotor.SetSpawnPoint(spawn);

            SpriteRenderer renderer = player.AddComponent<SpriteRenderer>();
            renderer.sprite = GetSquareSprite();
            renderer.color = new Color(0.45f, 1f, 0.86f, 0.95f);
            player.transform.localScale = new Vector3(0.7f, 1.2f, 1f);

            return player;
        }

        private void CreateCamera(Transform target)
        {
            Camera cameraToUse = sceneCamera != null ? sceneCamera : Camera.main;
            if (cameraToUse == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                cameraToUse = cameraObject.AddComponent<Camera>();
                cameraObject.AddComponent<AudioListener>();
            }

            cameraToUse.orthographic = true;
            cameraToUse.orthographicSize = 5.25f;
            cameraToUse.backgroundColor = new Color(0.07f, 0.085f, 0.095f);
            cameraToUse.transform.position = new Vector3(-15f, 0f, -10f);

            SimpleCameraFollow2D follow = cameraToUse.GetComponent<SimpleCameraFollow2D>();
            if (follow == null)
            {
                follow = cameraToUse.gameObject.AddComponent<SimpleCameraFollow2D>();
            }

            follow.Configure(target);
        }

        private void CreateTestPlatforms(Sprite sprite)
        {
            CreateBox(sprite, "AnimationTest_MainPlatform_80m", new Vector2(5f, -3f), new Vector2(80f, 1f), new Color(0.17f, 0.22f, 0.24f));
            CreateBox(sprite, "Left_TurnBuffer", new Vector2(-34.5f, -1.95f), new Vector2(1f, 3.1f), new Color(0.2f, 0.28f, 0.29f));
            CreateBox(sprite, "Right_TurnBuffer", new Vector2(44.5f, -1.95f), new Vector2(1f, 3.1f), new Color(0.2f, 0.28f, 0.29f));

            CreateBox(sprite, "JumpMarker_Low", new Vector2(-6f, -1.95f), new Vector2(1.8f, 0.35f), new Color(0.24f, 0.32f, 0.29f));
            CreateBox(sprite, "JumpMarker_Mid", new Vector2(-1.5f, -1.15f), new Vector2(1.8f, 0.35f), new Color(0.24f, 0.32f, 0.29f));
            CreateBox(sprite, "JumpMarker_High", new Vector2(3f, -0.35f), new Vector2(1.8f, 0.35f), new Color(0.24f, 0.32f, 0.29f));

            CreateVisual(sprite, "RunRhythm_Marker_A", new Vector2(14f, -2.42f), new Vector2(0.12f, 0.18f), new Color(0.55f, 1f, 0.86f, 0.85f));
            CreateVisual(sprite, "RunRhythm_Marker_B", new Vector2(18f, -2.42f), new Vector2(0.12f, 0.18f), new Color(0.55f, 1f, 0.86f, 0.85f));
            CreateVisual(sprite, "RunRhythm_Marker_C", new Vector2(22f, -2.42f), new Vector2(0.12f, 0.18f), new Color(0.55f, 1f, 0.86f, 0.85f));
        }

        private void CreateRespawnZone()
        {
            GameObject zone = new GameObject("RespawnZone_AnimationTestFallReset");
            zone.transform.position = new Vector3(5f, -8.5f, 0f);

            BoxCollider2D collider = zone.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(90f, 1f);
            collider.isTrigger = true;

            zone.AddComponent<RespawnZone2D>();
        }

        private static void CreateBox(Sprite sprite, string name, Vector2 position, Vector2 size, Color color)
        {
            GameObject box = new GameObject(name);
            box.transform.position = position;
            box.transform.localScale = size;

            SpriteRenderer renderer = box.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;

            BoxCollider2D collider = box.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
        }

        private static void CreateVisual(Sprite sprite, string name, Vector2 position, Vector2 size, Color color)
        {
            GameObject visual = new GameObject(name);
            visual.transform.position = position;
            visual.transform.localScale = size;

            SpriteRenderer renderer = visual.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = 5;
        }

        private static Sprite GetSquareSprite()
        {
            if (squareSprite != null)
            {
                return squareSprite;
            }

            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();

            squareSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            squareSprite.name = "Runtime_AnimationTestSquare";
            return squareSprite;
        }
    }
}
