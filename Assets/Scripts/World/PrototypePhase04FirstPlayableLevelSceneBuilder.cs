using Jade.CameraTools;
using Jade.Player;
using UnityEngine;

namespace Jade.World
{
    public class PrototypePhase04FirstPlayableLevelSceneBuilder : MonoBehaviour
    {
        [SerializeField] private PlayerMovementSettings movementSettings;
        [SerializeField] private Camera sceneCamera;

        private const string PlayerPrefabResourcePath = "Prefabs/Player/JadeSpiritPlayer";
        private const string LegacyPlayerPrefabResourcePath = "Prefabs/Player/JadeQilinPlayer";
        private static Sprite squareSprite;
        private static Sprite backgroundSprite;

        private void Awake()
        {
            BuildScene();
        }

        private void BuildScene()
        {
            Sprite sprite = GetSquareSprite();
            GameObject player = CreatePlayer();
            Camera cameraToUse = CreateCamera(player.transform);
            CreateBackground(cameraToUse);
            CreateRoute(sprite);
            CreateEnvironmentArtPass(sprite);
            CreateDashAbilityPickup(sprite, new Vector2(2.5f, -2.25f));
            CreateDoubleJumpAbilityPickup(sprite, new Vector2(31f, -0.45f));
            CreateCheckpoint(sprite, "Checkpoint_FirstShrine", new Vector2(46f, -0.75f));
            CreateCollectibles(sprite);
            CreateGoal(sprite, new Vector2(88f, 3f));
            CreateRespawnZone();
        }

        private GameObject CreatePlayer()
        {
            Vector3 spawn = new Vector3(-13f, -1.7f, 0f);
            GameObject prefab = Resources.Load<GameObject>(PlayerPrefabResourcePath);
            if (prefab == null)
            {
                prefab = Resources.Load<GameObject>(LegacyPlayerPrefabResourcePath);
            }

            if (prefab != null)
            {
                GameObject playerInstance = Instantiate(prefab, spawn, Quaternion.identity);
                playerInstance.name = "Player_Phase04_FirstPlayable";

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

            GameObject player = new GameObject("Player_Phase04_Fallback");
            player.transform.position = spawn;

            Rigidbody2D body = player.AddComponent<Rigidbody2D>();
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;

            BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.68f, 1.25f);

            player.AddComponent<PlayerInputReader>();
            player.AddComponent<PlayerAbilityInventory2D>();
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
            cameraToUse.orthographicSize = 6f;
            cameraToUse.backgroundColor = new Color(0.065f, 0.08f, 0.09f);
            cameraToUse.transform.position = new Vector3(-9f, 0f, -10f);

            SimpleCameraFollow2D follow = cameraToUse.GetComponent<SimpleCameraFollow2D>();
            if (follow == null)
            {
                follow = cameraToUse.gameObject.AddComponent<SimpleCameraFollow2D>();
            }

            follow.Configure(target);
            return cameraToUse;
        }

        private void CreateRoute(Sprite sprite)
        {
            CreateBox(sprite, "Start_SafeTrainingPlatform", new Vector2(-9f, -3f), new Vector2(13f, 1f), PlatformColor(RouteBand.Start));
            CreateBox(sprite, "RunRead_LeadIn", new Vector2(1f, -3f), new Vector2(7.5f, 0.55f), PlatformColor(RouteBand.Main));

            CreateBox(sprite, "DashTutorial_Takeoff", new Vector2(7.5f, -2.7f), new Vector2(2.2f, 0.45f), PlatformColor(RouteBand.Ability));
            CreateBox(sprite, "DashTutorial_Landing", new Vector2(18.2f, -2.25f), new Vector2(3f, 0.5f), PlatformColor(RouteBand.Ability));
            CreateBox(sprite, "ShortJump_01", new Vector2(21f, -1.95f), new Vector2(2.6f, 0.45f), PlatformColor(RouteBand.Jump));
            CreateBox(sprite, "ShortJump_02", new Vector2(26f, -1.45f), new Vector2(2.35f, 0.45f), PlatformColor(RouteBand.Jump));
            CreateBox(sprite, "ShortJump_03", new Vector2(31f, -1.15f), new Vector2(2.5f, 0.45f), PlatformColor(RouteBand.Jump));
            CreateBox(sprite, "DoubleJump_TutorialLanding", new Vector2(38.2f, 0.9f), new Vector2(3.2f, 0.45f), PlatformColor(RouteBand.Ability));
            CreateBox(sprite, "DoubleJump_ReturnToMain", new Vector2(42.8f, -1.25f), new Vector2(2.5f, 0.45f), PlatformColor(RouteBand.Ability));

            CreateBox(sprite, "BranchChoice_Main", new Vector2(46f, -1.25f), new Vector2(5f, 0.55f), PlatformColor(RouteBand.Main));
            CreateBox(sprite, "LowerBranch_DropCatch", new Vector2(36f, -5.15f), new Vector2(4.2f, 0.45f), PlatformColor(RouteBand.Optional));
            CreateBox(sprite, "LowerBranch_JadeRun", new Vector2(43f, -5.15f), new Vector2(6f, 0.45f), PlatformColor(RouteBand.Optional));
            CreateBox(sprite, "LowerBranch_ReturnStep_01", new Vector2(50f, -4.05f), new Vector2(2.4f, 0.45f), PlatformColor(RouteBand.Optional));
            CreateBox(sprite, "LowerBranch_ReturnStep_02", new Vector2(54.5f, -2.65f), new Vector2(2.4f, 0.45f), PlatformColor(RouteBand.Optional));

            CreateBox(sprite, "Checkpoint_MainPlatform", new Vector2(46f, -1.75f), new Vector2(9f, 0.55f), PlatformColor(RouteBand.Checkpoint));
            CreateBox(sprite, "AfterCheckpoint_Breath", new Vector2(57f, -1.25f), new Vector2(5.2f, 0.55f), PlatformColor(RouteBand.Main));

            CreateBox(sprite, "ClimbStep_01", new Vector2(63f, -0.35f), new Vector2(2.4f, 0.45f), PlatformColor(RouteBand.Climb));
            CreateBox(sprite, "ClimbStep_02", new Vector2(68f, 0.65f), new Vector2(2.4f, 0.45f), PlatformColor(RouteBand.Climb));
            CreateBox(sprite, "ClimbStep_03", new Vector2(73f, 1.6f), new Vector2(2.4f, 0.45f), PlatformColor(RouteBand.Climb));
            CreateBox(sprite, "ClimbStep_04", new Vector2(78f, 2.4f), new Vector2(2.6f, 0.45f), PlatformColor(RouteBand.Climb));

            CreateBox(sprite, "Final_Runup", new Vector2(84f, 2.25f), new Vector2(7f, 0.55f), PlatformColor(RouteBand.Goal));
            CreateBox(sprite, "Goal_Platform", new Vector2(91f, 2.25f), new Vector2(4f, 0.55f), PlatformColor(RouteBand.Goal));

            CreateHazardMarker(sprite, "FallZone_VisualGuide", new Vector2(43f, -7.1f), new Vector2(65f, 0.12f));
        }

        private void CreateEnvironmentArtPass(Sprite sprite)
        {
            PrototypePhase04EnvironmentArtPass2D artPass = GetComponent<PrototypePhase04EnvironmentArtPass2D>();
            if (artPass == null)
            {
                artPass = gameObject.AddComponent<PrototypePhase04EnvironmentArtPass2D>();
            }

            artPass.Build(sprite);
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
            renderer.color = new Color(0.7f, 0.82f, 0.82f, 0.68f);
            renderer.sortingOrder = -30;

            ScaleBackgroundToCamera(panel.transform, sprite, cameraToUse, 1.08f);
        }

        private void CreateCheckpoint(Sprite sprite, string name, Vector2 position)
        {
            GameObject checkpoint = new GameObject(name);
            checkpoint.transform.position = position;

            BoxCollider2D trigger = checkpoint.AddComponent<BoxCollider2D>();
            trigger.size = new Vector2(1.3f, 0.85f);
            trigger.offset = new Vector2(0f, 0.55f);
            trigger.isTrigger = true;

            SpriteRenderer marker = CreateChildVisual(sprite, "Marker", checkpoint.transform, Vector2.zero, new Vector2(0.35f, 1.6f), new Color(0.25f, 0.45f, 0.42f, 0.75f), 15);
            Checkpoint2D checkpointComponent = checkpoint.AddComponent<Checkpoint2D>();
            checkpointComponent.Configure(checkpoint.transform, marker);
        }

        private void CreateCollectibles(Sprite sprite)
        {
            CreateCollectible(sprite, "Collectible_LowerBranch_01", new Vector2(36f, -4.45f));
            CreateCollectible(sprite, "Collectible_LowerBranch_02", new Vector2(43f, -4.45f));
            CreateCollectible(sprite, "Collectible_ReturnStep_Reward", new Vector2(54.5f, -1.95f));
        }

        private void CreateDashAbilityPickup(Sprite sprite, Vector2 position)
        {
            GameObject pickup = new GameObject("AbilityPickup_AirDash");
            pickup.transform.position = position;

            CircleCollider2D trigger = pickup.AddComponent<CircleCollider2D>();
            trigger.radius = 0.45f;
            trigger.isTrigger = true;

            SpriteRenderer visual = pickup.AddComponent<SpriteRenderer>();
            visual.sprite = sprite;
            visual.color = new Color(0.62f, 1f, 0.95f, 1f);
            visual.sortingOrder = 18;
            pickup.transform.localScale = new Vector3(0.42f, 0.42f, 1f);

            DashAbilityPickup2D pickupComponent = pickup.AddComponent<DashAbilityPickup2D>();
            pickupComponent.Configure(visual);
        }

        private void CreateDoubleJumpAbilityPickup(Sprite sprite, Vector2 position)
        {
            GameObject pickup = new GameObject("AbilityPickup_DoubleJump");
            pickup.transform.position = position;

            CircleCollider2D trigger = pickup.AddComponent<CircleCollider2D>();
            trigger.radius = 0.45f;
            trigger.isTrigger = true;

            SpriteRenderer visual = pickup.AddComponent<SpriteRenderer>();
            visual.sprite = sprite;
            visual.color = new Color(0.72f, 0.95f, 1f, 1f);
            visual.sortingOrder = 18;
            pickup.transform.localScale = new Vector3(0.42f, 0.42f, 1f);

            DoubleJumpAbilityPickup2D pickupComponent = pickup.AddComponent<DoubleJumpAbilityPickup2D>();
            pickupComponent.Configure(visual);
        }

        private void CreateCollectible(Sprite sprite, string name, Vector2 position)
        {
            GameObject collectible = new GameObject(name);
            collectible.transform.position = position;

            CircleCollider2D trigger = collectible.AddComponent<CircleCollider2D>();
            trigger.radius = 0.35f;
            trigger.isTrigger = true;

            SpriteRenderer visual = collectible.AddComponent<SpriteRenderer>();
            visual.sprite = sprite;
            visual.color = new Color(0.35f, 1f, 0.85f, 0.95f);
            visual.sortingOrder = 16;
            collectible.transform.localScale = new Vector3(0.28f, 0.28f, 1f);

            Collectible2D collectibleComponent = collectible.AddComponent<Collectible2D>();
            collectibleComponent.Configure(visual);
        }

        private void CreateGoal(Sprite sprite, Vector2 position)
        {
            GameObject goal = new GameObject("LevelGoal_FirstPlayable_EndMarker");
            goal.transform.position = position;

            BoxCollider2D trigger = goal.AddComponent<BoxCollider2D>();
            trigger.size = new Vector2(1.4f, 2.3f);
            trigger.offset = new Vector2(0f, 1.15f);
            trigger.isTrigger = true;

            SpriteRenderer marker = CreateChildVisual(sprite, "Goal_JadeGate", goal.transform, new Vector2(0f, 1.05f), new Vector2(0.6f, 2.1f), new Color(0.45f, 0.9f, 0.8f, 0.9f), 15);
            LevelGoal2D goalComponent = goal.AddComponent<LevelGoal2D>();
            goalComponent.Configure(marker);
        }

        private void CreateRespawnZone()
        {
            GameObject zone = new GameObject("RespawnZone_Phase04FallReset");
            zone.transform.position = new Vector3(33f, -10.5f, 0f);

            BoxCollider2D collider = zone.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(115f, 1f);
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

        private static void CreateHazardMarker(Sprite sprite, string name, Vector2 position, Vector2 size)
        {
            GameObject marker = new GameObject(name);
            marker.transform.position = position;
            marker.transform.localScale = size;

            SpriteRenderer renderer = marker.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(0.7f, 0.18f, 0.18f, 0.35f);
            renderer.sortingOrder = -1;
        }

        private static SpriteRenderer CreateChildVisual(Sprite sprite, string name, Transform parent, Vector2 localPosition, Vector2 scale, Color color, int sortingOrder)
        {
            GameObject visual = new GameObject(name);
            visual.transform.SetParent(parent);
            visual.transform.localPosition = localPosition;
            visual.transform.localScale = scale;

            SpriteRenderer renderer = visual.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        private static Color PlatformColor(RouteBand band)
        {
            switch (band)
            {
                case RouteBand.Start:
                    return new Color(0.18f, 0.22f, 0.24f);
                case RouteBand.Jump:
                    return new Color(0.23f, 0.28f, 0.3f);
                case RouteBand.Ability:
                    return new Color(0.18f, 0.34f, 0.34f);
                case RouteBand.Optional:
                    return new Color(0.14f, 0.2f, 0.24f);
                case RouteBand.Checkpoint:
                    return new Color(0.18f, 0.31f, 0.28f);
                case RouteBand.Climb:
                    return new Color(0.24f, 0.25f, 0.22f);
                case RouteBand.Goal:
                    return new Color(0.3f, 0.25f, 0.2f);
                default:
                    return new Color(0.2f, 0.3f, 0.28f);
            }
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
            squareSprite.name = "Runtime_Phase04Square";
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

        private enum RouteBand
        {
            Start,
            Main,
            Jump,
            Ability,
            Optional,
            Checkpoint,
            Climb,
            Goal
        }
    }
}
