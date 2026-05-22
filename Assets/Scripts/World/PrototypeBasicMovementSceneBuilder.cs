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
        private static Sprite backgroundSprite;
        private const string PlayerPrefabResourcePath = "Prefabs/Player/JadeSpiritPlayer";
        private const string LegacyPlayerPrefabResourcePath = "Prefabs/Player/JadeQilinPlayer";
        private static readonly string[] CharacterFrameNames =
        {
            "Idle01",
            "Idle02",
            "Run01",
            "Run02",
            "Run03",
            "Run04",
            "Jump",
            "Fall"
        };

        private void Awake()
        {
            BuildScene();
        }

        private void BuildScene()
        {
            Sprite sprite = GetSquareSprite();
            GameObject player = CreatePlayer(sprite);
            Camera cameraToUse = CreateCamera(player.transform);
            CreateBackground(cameraToUse);
            CreatePlatforms(sprite);
            CreateRespawnZone();
        }

        private GameObject CreatePlayer(Sprite sprite)
        {
            Vector3 spawn = new Vector3(-8f, -1.7f, 0f);
            GameObject prefab = Resources.Load<GameObject>(PlayerPrefabResourcePath);
            if (prefab == null)
            {
                prefab = Resources.Load<GameObject>(LegacyPlayerPrefabResourcePath);
            }
            if (prefab != null)
            {
                GameObject playerInstance = Instantiate(prefab, spawn, Quaternion.identity);
                playerInstance.name = "Player_BasicMovement";

                PlayerMotor2D prefabMotor = playerInstance.GetComponent<PlayerMotor2D>();
                if (prefabMotor != null)
                {
                    prefabMotor.Configure(movementSettings);
                    prefabMotor.SetSpawnPoint(spawn);
                }

                return playerInstance;
            }

            GameObject player = new GameObject("Player_BasicMovement");
            player.transform.position = spawn;

            Rigidbody2D body = player.AddComponent<Rigidbody2D>();
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;

            BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.68f, 1.25f);

            player.AddComponent<PlayerInputReader>();
            player.AddComponent<PlayerAbilityInventory2D>();
            PlayerMotor2D motor = player.AddComponent<PlayerMotor2D>();
            motor.Configure(movementSettings);
            motor.SetSpawnPoint(spawn);

            CreatePlayerVisuals(player, sprite);

            return player;
        }

        private void CreatePlayerVisuals(GameObject player, Sprite sprite)
        {
            GameObject visualRoot = new GameObject("VisualRoot_TemporaryCharacter");
            visualRoot.transform.SetParent(player.transform);
            visualRoot.transform.localPosition = Vector3.zero;

            SpriteRenderer characterRenderer = CreateAnimatedCharacterVisual(visualRoot.transform);
            if (characterRenderer != null)
            {
                PlayerSpriteAnimator2D animator = player.AddComponent<PlayerSpriteAnimator2D>();
                animator.Configure(characterRenderer, LoadCharacterFrameSprites());
            }
            else
            {
                CreateSpiritAnimalProxy(sprite, visualRoot.transform);
            }

            ParticleSystem landingDust = CreateDust("LandingDust", player.transform, new Vector3(0f, -0.78f, 0f), false);
            ParticleSystem runDust = CreateDust("RunDust", player.transform, new Vector3(-0.25f, -0.78f, 0f), true);

            PlayerVisualFeedback2D feedback = player.AddComponent<PlayerVisualFeedback2D>();
            feedback.Configure(visualRoot.transform, landingDust, runDust, null);
        }

        private Camera CreateCamera(Transform target)
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
            return cameraToUse;
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

        private static void CreateBackground(Camera cameraToUse)
        {
            Sprite sprite = GetBackgroundSprite();
            if (sprite == null || cameraToUse == null)
            {
                return;
            }

            GameObject panel = new GameObject("Background_JadeMythForest_CameraLayer");
            panel.transform.SetParent(cameraToUse.transform);
            panel.transform.localPosition = new Vector3(0f, 0.15f, 12f);
            panel.transform.localRotation = Quaternion.identity;

            SpriteRenderer renderer = panel.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(0.74f, 0.82f, 0.82f, 0.72f);
            renderer.sortingOrder = -30;

            ScaleBackgroundToCamera(panel.transform, sprite, cameraToUse, 1.08f);
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

        private static void CreateVisualPart(Sprite sprite, string name, Transform parent, Vector2 localPosition, Vector2 scale, Color color, int sortingOrder)
        {
            GameObject part = new GameObject(name);
            part.transform.SetParent(parent);
            part.transform.localPosition = localPosition;
            part.transform.localScale = scale;

            SpriteRenderer renderer = part.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
        }

        private static SpriteRenderer CreateAnimatedCharacterVisual(Transform visualRoot)
        {
            Sprite[] frameSprites = LoadCharacterFrameSprites();
            if (frameSprites == null)
            {
                return null;
            }

            GameObject spriteObject = new GameObject("JadeSpirit_FrameSprite");
            spriteObject.transform.SetParent(visualRoot);
            spriteObject.transform.localPosition = new Vector3(0f, -0.68f, 0f);
            spriteObject.transform.localScale = Vector3.one * GetSourceScaleCompensation(frameSprites[0]);

            SpriteRenderer renderer = spriteObject.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = 16;
            return renderer;
        }

        private static Sprite[] LoadCharacterFrameSprites()
        {
            Sprite[] sprites = new Sprite[CharacterFrameNames.Length];
            for (int i = 0; i < CharacterFrameNames.Length; i++)
            {
                sprites[i] = Resources.Load<Sprite>("Characters/JadeQilinFrames/JadeQilin_" + CharacterFrameNames[i]);
                if (sprites[i] == null)
                {
                    return null;
                }
            }

            return sprites;
        }

        private static float GetSourceScaleCompensation(Sprite sprite)
        {
            if (sprite == null || sprite.rect.height >= 1024f)
            {
                return 1f;
            }

            return 1024f / Mathf.Max(sprite.rect.height, 1f);
        }

        private static void CreateSpiritAnimalProxy(Sprite sprite, Transform visualRoot)
        {
            Color ink = new Color(0.12f, 0.24f, 0.23f);
            Color deepInk = new Color(0.06f, 0.12f, 0.13f);
            Color jade = new Color(0.4f, 1f, 0.86f, 0.95f);
            Color paleJade = new Color(0.72f, 1f, 0.92f, 0.95f);

            CreateVisualPart(sprite, "Tail_Ribbon_Long", visualRoot, new Vector2(-0.58f, 0.12f), new Vector2(0.86f, 0.14f), new Color(0.42f, 1f, 0.9f, 0.62f), 10);
            CreateVisualPart(sprite, "Tail_Ribbon_Tip", visualRoot, new Vector2(-0.94f, 0.28f), new Vector2(0.34f, 0.1f), new Color(0.62f, 1f, 0.94f, 0.65f), 10);
            CreateVisualPart(sprite, "Tail_Ink_Base", visualRoot, new Vector2(-0.32f, 0.04f), new Vector2(0.36f, 0.1f), deepInk, 11);
            CreateVisualPart(sprite, "Body_JadeInk", visualRoot, new Vector2(0f, -0.12f), new Vector2(0.68f, 0.46f), ink, 12);
            CreateVisualPart(sprite, "Belly_Mist", visualRoot, new Vector2(0.08f, -0.2f), new Vector2(0.44f, 0.18f), new Color(0.56f, 0.9f, 0.78f, 0.55f), 13);
            CreateVisualPart(sprite, "Chest_JadeCore", visualRoot, new Vector2(0.26f, 0.02f), new Vector2(0.15f, 0.22f), jade, 15);
            CreateVisualPart(sprite, "Head_JadeInk", visualRoot, new Vector2(0.39f, 0.31f), new Vector2(0.4f, 0.32f), ink, 13);
            CreateVisualPart(sprite, "Muzzle_Glow", visualRoot, new Vector2(0.62f, 0.25f), new Vector2(0.22f, 0.14f), new Color(0.5f, 0.95f, 0.82f, 0.72f), 14);
            CreateVisualPart(sprite, "Eye_Jade", visualRoot, new Vector2(0.53f, 0.38f), new Vector2(0.08f, 0.07f), paleJade, 16);
            CreateVisualPart(sprite, "Ear_Back", visualRoot, new Vector2(0.22f, 0.58f), new Vector2(0.12f, 0.32f), new Color(0.16f, 0.36f, 0.32f), 12);
            CreateVisualPart(sprite, "Ear_Front_Glow", visualRoot, new Vector2(0.43f, 0.58f), new Vector2(0.1f, 0.3f), new Color(0.48f, 0.98f, 0.84f, 0.9f), 15);
            CreateVisualPart(sprite, "Horn_Jade_A", visualRoot, new Vector2(0.35f, 0.76f), new Vector2(0.07f, 0.24f), paleJade, 16);
            CreateVisualPart(sprite, "Horn_Jade_B", visualRoot, new Vector2(0.49f, 0.73f), new Vector2(0.06f, 0.2f), paleJade, 16);
            CreateVisualPart(sprite, "Leg_Front", visualRoot, new Vector2(0.24f, -0.46f), new Vector2(0.1f, 0.34f), deepInk, 12);
            CreateVisualPart(sprite, "Leg_Back", visualRoot, new Vector2(-0.24f, -0.46f), new Vector2(0.1f, 0.34f), deepInk, 12);
            CreateVisualPart(sprite, "Hoof_Front_Glow", visualRoot, new Vector2(0.24f, -0.66f), new Vector2(0.14f, 0.06f), jade, 15);
            CreateVisualPart(sprite, "Hoof_Back_Glow", visualRoot, new Vector2(-0.24f, -0.66f), new Vector2(0.14f, 0.06f), jade, 15);
            CreateVisualPart(sprite, "Foot_Shadow", visualRoot, new Vector2(0f, -0.72f), new Vector2(0.78f, 0.06f), new Color(0.03f, 0.04f, 0.04f, 0.48f), 9);
        }

        private static ParticleSystem CreateDust(string name, Transform parent, Vector3 localPosition, bool loop)
        {
            GameObject dustObject = new GameObject(name);
            dustObject.transform.SetParent(parent);
            dustObject.transform.localPosition = localPosition;

            ParticleSystem particles = dustObject.AddComponent<ParticleSystem>();
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            ParticleSystem.MainModule main = particles.main;
            main.duration = loop ? 0.6f : 0.25f;
            main.loop = loop;
            main.startLifetime = loop ? 0.25f : 0.32f;
            main.startSpeed = loop ? 0.6f : 1.6f;
            main.startSize = loop ? 0.08f : 0.16f;
            main.startColor = new Color(0.65f, 0.78f, 0.72f, 0.38f);
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            ParticleSystem.EmissionModule emission = particles.emission;
            emission.enabled = loop;
            emission.rateOverTime = loop ? 12f : 0f;
            emission.SetBursts(loop ? new ParticleSystem.Burst[0] : new[] { new ParticleSystem.Burst(0f, 10) });

            ParticleSystem.ShapeModule shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 25f;
            shape.radius = 0.18f;

            ParticleSystem.VelocityOverLifetimeModule velocity = particles.velocityOverLifetime;
            velocity.enabled = false;

            ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
            renderer.sortingOrder = 8;
            renderer.material = new Material(Shader.Find("Sprites/Default"));

            if (!loop)
            {
                particles.Stop();
            }

            return particles;
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

        private static Sprite GetBackgroundSprite()
        {
            if (backgroundSprite != null)
            {
                return backgroundSprite;
            }

            Texture2D texture = Resources.Load<Texture2D>("Backgrounds/JadeMythForest_Background");
            if (texture == null)
            {
                return null;
            }

            backgroundSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
            backgroundSprite.name = "JadeMythForest_Background_RuntimeSprite";
            return backgroundSprite;
        }

        private static void ScaleBackgroundToCamera(Transform background, Sprite sprite, Camera cameraToUse, float overscan)
        {
            float cameraHeight = cameraToUse.orthographicSize * 2f * overscan;
            float aspect = cameraToUse.aspect > 0f ? cameraToUse.aspect : 16f / 9f;
            float cameraWidth = cameraHeight * aspect;
            Vector2 spriteSize = sprite.bounds.size;
            float scale = Mathf.Max(cameraWidth / spriteSize.x, cameraHeight / spriteSize.y);
            background.localScale = new Vector3(scale, scale, 1f);
        }

    }
}
