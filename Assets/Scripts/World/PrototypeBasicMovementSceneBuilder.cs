using Jade.CameraTools;
using Jade.Player;
using UnityEngine;

namespace Jade.World
{
    public class PrototypeBasicMovementSceneBuilder : MonoBehaviour
    {
        [SerializeField] private PlayerMovementSettings movementSettings;
        [SerializeField] private Camera sceneCamera;

        private static Sprite squareSprite;

        private void Awake()
        {
            BuildScene();
        }

        private void BuildScene()
        {
            Sprite sprite = GetSquareSprite();
            GameObject player = CreatePlayer(sprite);
            CreateCamera(player.transform);
            CreatePlatforms(sprite);
            CreateRespawnZone();
        }

        private GameObject CreatePlayer(Sprite sprite)
        {
            GameObject player = new GameObject("Player_BasicMovement");
            player.transform.position = new Vector3(-8f, -1.7f, 0f);

            SpriteRenderer renderer = player.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(0.35f, 0.95f, 0.9f);
            renderer.sortingOrder = 10;

            Rigidbody2D body = player.AddComponent<Rigidbody2D>();
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;

            BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.75f, 1.5f);

            player.AddComponent<PlayerInputReader>();
            PlayerMotor2D motor = player.AddComponent<PlayerMotor2D>();
            motor.Configure(movementSettings);
            motor.SetSpawnPoint(player.transform.position);

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
            cameraToUse.orthographicSize = 5.5f;
            cameraToUse.backgroundColor = new Color(0.08f, 0.1f, 0.12f);
            cameraToUse.transform.position = new Vector3(-5f, 0f, -10f);

            SimpleCameraFollow2D follow = cameraToUse.GetComponent<SimpleCameraFollow2D>();
            if (follow == null)
            {
                follow = cameraToUse.gameObject.AddComponent<SimpleCameraFollow2D>();
            }

            follow.Configure(target);
        }

        private void CreatePlatforms(Sprite sprite)
        {
            CreateBox(sprite, "Ground_LongRun", new Vector2(-2f, -3f), new Vector2(18f, 1f), new Color(0.18f, 0.22f, 0.24f));
            CreateBox(sprite, "Platform_EdgeBuffer_A", new Vector2(10.5f, -1.4f), new Vector2(3f, 0.45f), new Color(0.23f, 0.28f, 0.3f));
            CreateBox(sprite, "Platform_EdgeBuffer_B", new Vector2(15f, -0.2f), new Vector2(2.5f, 0.45f), new Color(0.23f, 0.28f, 0.3f));
            CreateBox(sprite, "Platform_JumpHeight_Low", new Vector2(20f, -1.6f), new Vector2(2.5f, 0.45f), new Color(0.2f, 0.3f, 0.28f));
            CreateBox(sprite, "Platform_JumpHeight_Mid", new Vector2(24f, 0.1f), new Vector2(2.5f, 0.45f), new Color(0.2f, 0.3f, 0.28f));
            CreateBox(sprite, "Platform_JumpHeight_High", new Vector2(28f, 1.8f), new Vector2(3.5f, 0.45f), new Color(0.2f, 0.3f, 0.28f));
        }

        private void CreateRespawnZone()
        {
            GameObject zone = new GameObject("RespawnZone_FallReset");
            zone.transform.position = new Vector3(10f, -8f, 0f);

            BoxCollider2D collider = zone.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(60f, 1f);
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
            squareSprite.name = "Runtime_PrototypeSquare";
            return squareSprite;
        }
    }
}
