using Jade.CameraTools;
using Jade.Player;
using UnityEngine;

namespace Jade.World
{
    public class ShanhaiGateGrayboxSceneBuilder : MonoBehaviour
    {
        [SerializeField] private PlayerMovementSettings movementSettings;
        [SerializeField] private Camera sceneCamera;

        private const int RequiredShardCount = 4;
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
            CreateAbilityPickups(sprite);
            CreateCheckpoints(sprite);
            CreateShards(sprite);
            CreateShardGate(sprite);
            CreateRespawnZone();
        }

        private GameObject CreatePlayer()
        {
            Vector3 spawn = new Vector3(-12f, -1.65f, 0f);
            GameObject prefab = Resources.Load<GameObject>(PlayerPrefabResourcePath);
            if (prefab == null)
            {
                prefab = Resources.Load<GameObject>(LegacyPlayerPrefabResourcePath);
            }

            GameObject player;
            if (prefab != null)
            {
                player = Instantiate(prefab, spawn, Quaternion.identity);
                player.name = "Player_ShanhaiGate_Graybox";
            }
            else
            {
                player = new GameObject("Player_ShanhaiGate_Fallback");
                player.transform.position = spawn;

                Rigidbody2D body = player.AddComponent<Rigidbody2D>();
                body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                body.interpolation = RigidbodyInterpolation2D.Interpolate;
                body.constraints = RigidbodyConstraints2D.FreezeRotation;

                BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(0.68f, 1.25f);

                player.AddComponent<PlayerInputReader>();
                player.AddComponent<PlayerAbilityInventory2D>();
                SpriteRenderer renderer = player.AddComponent<SpriteRenderer>();
                renderer.sprite = GetSquareSprite();
                renderer.color = new Color(0.45f, 1f, 0.86f, 0.95f);
                player.transform.localScale = new Vector3(0.7f, 1.2f, 1f);
            }

            PlayerMotor2D motor = player.GetComponent<PlayerMotor2D>();
            if (motor != null)
            {
                if (movementSettings != null)
                {
                    motor.Configure(movementSettings);
                }

                motor.SetSpawnPoint(spawn);
            }

            ShanhaiShardInventory2D shardInventory = player.GetComponent<ShanhaiShardInventory2D>();
            if (shardInventory == null)
            {
                shardInventory = player.AddComponent<ShanhaiShardInventory2D>();
            }

            shardInventory.Configure(RequiredShardCount);
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
            cameraToUse.backgroundColor = new Color(0.06f, 0.08f, 0.085f);
            cameraToUse.transform.position = new Vector3(-8f, 0f, -10f);

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
            // Segment 1: movement, hazards, dash gate, and double-jump pickup.
            CreateBox(sprite, "S1_Start_SafeRun", new Vector2(-7f, -3f), new Vector2(14f, 1f), PlatformColor(RouteBand.Start));
            CreateHazard(sprite, "S1_FireRead_Hazard", new Vector2(0.2f, -2.25f), new Vector2(0.8f, 0.75f), HazardColor(HazardBand.Fire));
            CreateBox(sprite, "S1_FirstHop", new Vector2(5f, -2.4f), new Vector2(3f, 0.45f), PlatformColor(RouteBand.Jump));
            CreateHazard(sprite, "S1_SpikeRead_Hazard", new Vector2(8.1f, -2.95f), new Vector2(2f, 0.5f), HazardColor(HazardBand.Spike));
            CreateBox(sprite, "S1_DashPickup_Shelf", new Vector2(11.5f, -2.25f), new Vector2(4f, 0.45f), PlatformColor(RouteBand.Ability));
            CreateBox(sprite, "S1_DashGate_Takeoff", new Vector2(16.3f, -2.35f), new Vector2(2.2f, 0.45f), PlatformColor(RouteBand.AbilityGate));
            CreateHazard(sprite, "S1_DashGate_Pit", new Vector2(21.3f, -3.8f), new Vector2(5.6f, 0.6f), HazardColor(HazardBand.Fall));
            CreateBox(sprite, "S1_DashGate_Landing", new Vector2(25.2f, -2.05f), new Vector2(3.2f, 0.5f), PlatformColor(RouteBand.AbilityGate));
            CreateBox(sprite, "S1_Optional_Lookahead_LockedHigh", new Vector2(26f, 2.4f), new Vector2(4.2f, 0.45f), PlatformColor(RouteBand.Return));
            CreateBox(sprite, "S1_MainBridge_ToDoubleJump", new Vector2(31.2f, -2.05f), new Vector2(6f, 0.5f), PlatformColor(RouteBand.Main));
            CreateBox(sprite, "S1_DoubleJumpPickup_Dais", new Vector2(38f, -1.35f), new Vector2(3.4f, 0.5f), PlatformColor(RouteBand.Ability));
            CreateBox(sprite, "S1_DoubleJumpGate_Rise01", new Vector2(44f, 0.35f), new Vector2(2.7f, 0.45f), PlatformColor(RouteBand.AbilityGate));
            CreateBox(sprite, "S1_DoubleJumpGate_Rise02", new Vector2(50f, 1.75f), new Vector2(2.7f, 0.45f), PlatformColor(RouteBand.AbilityGate));
            CreateBox(sprite, "S1_ReturnShard_EarlyTease", new Vector2(31.3f, 2.55f), new Vector2(4f, 0.45f), PlatformColor(RouteBand.Shard));

            // Segment 2: lake, bamboo jumps, hazards, and final climb approach.
            CreateBox(sprite, "S2_LakeEntry_CheckpointPlatform", new Vector2(57f, 1.45f), new Vector2(7f, 0.55f), PlatformColor(RouteBand.Checkpoint));
            CreateHazard(sprite, "S2_LakeWater_Hazard", new Vector2(70f, -2.6f), new Vector2(23f, 0.55f), HazardColor(HazardBand.Water));
            CreateBox(sprite, "S2_LakeSkip_01", new Vector2(65f, -0.45f), new Vector2(2.3f, 0.35f), PlatformColor(RouteBand.Jump));
            CreateBox(sprite, "S2_LakeSkip_02", new Vector2(70f, -0.25f), new Vector2(2.2f, 0.35f), PlatformColor(RouteBand.Jump));
            CreateBox(sprite, "S2_LakeSkip_03", new Vector2(75.4f, 0.1f), new Vector2(2.3f, 0.35f), PlatformColor(RouteBand.Jump));
            CreateBox(sprite, "S2_BambooPillar_01", new Vector2(82f, -1.25f), new Vector2(1f, 4f), PlatformColor(RouteBand.Bamboo));
            CreateBox(sprite, "S2_BambooPillar_02", new Vector2(88f, -0.4f), new Vector2(1f, 5.7f), PlatformColor(RouteBand.Bamboo));
            CreateBox(sprite, "S2_BambooPillar_03", new Vector2(94f, 0.45f), new Vector2(1f, 7.4f), PlatformColor(RouteBand.Bamboo));
            CreateBox(sprite, "S2_BambooTop_Reward", new Vector2(94f, 4.35f), new Vector2(3.3f, 0.4f), PlatformColor(RouteBand.Shard));
            CreateBox(sprite, "S2_SwingPole_Anchor", new Vector2(101f, 2.1f), new Vector2(0.55f, 5.5f), PlatformColor(RouteBand.Mechanism));
            CreateBox(sprite, "S2_SwingPole_Blade", new Vector2(103.2f, 1.65f), new Vector2(4.7f, 0.28f), PlatformColor(RouteBand.Mechanism));
            CreateHazard(sprite, "S2_SwingPole_Hazard", new Vector2(103.2f, 1.65f), new Vector2(4.7f, 0.28f), HazardColor(HazardBand.Mechanism));
            CreateBox(sprite, "S2_FinalClimbApproach_Platform", new Vector2(110f, 0.75f), new Vector2(5.6f, 0.55f), PlatformColor(RouteBand.Main));
            CreateHazard(sprite, "S2_BambooSpike_Hazard", new Vector2(115.2f, 1.35f), new Vector2(2f, 0.75f), HazardColor(HazardBand.Spike));
            CreateBox(sprite, "S2_ExitBeforeFinalClimb", new Vector2(119f, 0.75f), new Vector2(5.5f, 0.55f), PlatformColor(RouteBand.Main));

            // Segment 3: double-jump climb, last shards, and four-shard gate.
            CreateBox(sprite, "S3_FinalClimb_Floor", new Vector2(127f, 0.75f), new Vector2(7f, 0.55f), PlatformColor(RouteBand.Checkpoint));
            CreateHazard(sprite, "S3_FinalClimb_BottomSpikes", new Vector2(134f, -0.1f), new Vector2(4.2f, 0.55f), HazardColor(HazardBand.Spike));
            CreateBox(sprite, "S3_FinalClimb_Step01", new Vector2(132.5f, 2.65f), new Vector2(3.2f, 0.45f), PlatformColor(RouteBand.AbilityGate));
            CreateBox(sprite, "S3_FinalClimb_Step02", new Vector2(138.5f, 4.45f), new Vector2(3.2f, 0.45f), PlatformColor(RouteBand.AbilityGate));
            CreateBox(sprite, "S3_FinalClimb_Step03", new Vector2(140.8f, 6.35f), new Vector2(3.1f, 0.45f), PlatformColor(RouteBand.AbilityGate));
            CreateBox(sprite, "S3_FinalClimb_Step04", new Vector2(133.4f, 9.45f), new Vector2(3.1f, 0.45f), PlatformColor(RouteBand.AbilityGate));
            CreateBox(sprite, "S3_UpperReturn_Ledge", new Vector2(144.5f, 11.6f), new Vector2(6f, 0.5f), PlatformColor(RouteBand.Return));
            CreateBox(sprite, "S3_DropCatch_ToFinal", new Vector2(151f, 5.4f), new Vector2(4.8f, 0.5f), PlatformColor(RouteBand.Main));
            CreateBox(sprite, "S3_FinalShard_Ledge", new Vector2(156.5f, 7.9f), new Vector2(4.2f, 0.45f), PlatformColor(RouteBand.Shard));
            CreateBox(sprite, "S3_GateApproach", new Vector2(164f, 5.4f), new Vector2(9.5f, 0.55f), PlatformColor(RouteBand.Goal));
            CreateBox(sprite, "S3_FinalAltar_Platform", new Vector2(177f, 5.4f), new Vector2(8f, 0.55f), PlatformColor(RouteBand.Goal));
        }

        private void CreateAbilityPickups(Sprite sprite)
        {
            CreateDashAbilityPickup(sprite, new Vector2(11.5f, -1.55f));
            CreateDoubleJumpAbilityPickup(sprite, new Vector2(38f, -0.65f));
        }

        private void CreateCheckpoints(Sprite sprite)
        {
            CreateCheckpoint(sprite, "Checkpoint_01_AfterDoubleJump", new Vector2(57f, 2f));
            CreateCheckpoint(sprite, "Checkpoint_02_FinalClimb", new Vector2(127f, 1.3f));
        }

        private void CreateShards(Sprite sprite)
        {
            CreateShard(sprite, "Shard_01_ReturnAfterDoubleJump", new Vector2(31.3f, 3.25f));
            CreateShard(sprite, "Shard_02_BambooCanopy", new Vector2(94f, 5.05f));
            CreateShard(sprite, "Shard_03_FinalClimbUpper", new Vector2(144.5f, 12.3f));
            CreateShard(sprite, "Shard_04_FinalHighLedge", new Vector2(156.5f, 8.6f));
        }

        private void CreateShardGate(Sprite sprite)
        {
            GameObject gate = new GameObject("ShanhaiGate_RequiresFourShards");
            gate.transform.position = new Vector3(173.4f, 5.65f, 0f);

            BoxCollider2D trigger = gate.AddComponent<BoxCollider2D>();
            trigger.size = new Vector2(2.5f, 3.2f);
            trigger.offset = new Vector2(-1.1f, 1.6f);
            trigger.isTrigger = true;

            GameObject solid = new GameObject("LockedGate_SolidCollider");
            solid.transform.SetParent(gate.transform);
            solid.transform.localPosition = new Vector3(0f, 1.65f, 0f);
            solid.transform.localScale = new Vector3(0.85f, 3.3f, 1f);

            SpriteRenderer gateRenderer = solid.AddComponent<SpriteRenderer>();
            gateRenderer.sprite = sprite;
            gateRenderer.color = new Color(0.55f, 0.18f, 0.18f, 0.9f);
            gateRenderer.sortingOrder = 14;

            BoxCollider2D solidCollider = solid.AddComponent<BoxCollider2D>();
            solidCollider.size = Vector2.one;

            SpriteRenderer goalRenderer = CreateChildVisual(
                sprite,
                "Goal_AltarMarker",
                gate.transform,
                new Vector2(4.1f, 1.15f),
                new Vector2(1.4f, 2.3f),
                new Color(0.4f, 0.58f, 0.62f, 0.9f),
                15);

            ShanhaiShardGate2D shardGate = gate.AddComponent<ShanhaiShardGate2D>();
            shardGate.Configure(RequiredShardCount, solidCollider, trigger, gateRenderer, goalRenderer);
        }

        private void CreateRespawnZone()
        {
            GameObject zone = new GameObject("RespawnZone_ShanhaiGate_FallReset");
            zone.transform.position = new Vector3(82f, -8.5f, 0f);

            BoxCollider2D collider = zone.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(210f, 1f);
            collider.isTrigger = true;

            zone.AddComponent<RespawnZone2D>();
        }

        private static void CreateBackground(Camera cameraToUse)
        {
            Sprite sprite = GetBackgroundSprite();
            if (sprite == null || cameraToUse == null)
            {
                return;
            }

            GameObject panel = new GameObject("Background_GrayboxReferenceLayer");
            panel.transform.SetParent(cameraToUse.transform);
            panel.transform.localPosition = new Vector3(0f, 0.15f, 12f);
            panel.transform.localRotation = Quaternion.identity;

            SpriteRenderer renderer = panel.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(0.58f, 0.72f, 0.72f, 0.42f);
            renderer.sortingOrder = -30;

            ScaleBackgroundToCamera(panel.transform, sprite, cameraToUse, 1.08f);
        }

        private void CreateDashAbilityPickup(Sprite sprite, Vector2 position)
        {
            GameObject pickup = CreatePickupBase(sprite, "AbilityPickup_01_AirDash", position, new Color(0.62f, 1f, 0.95f, 1f));
            pickup.AddComponent<DashAbilityPickup2D>().Configure(pickup.GetComponent<SpriteRenderer>());
        }

        private void CreateDoubleJumpAbilityPickup(Sprite sprite, Vector2 position)
        {
            GameObject pickup = CreatePickupBase(sprite, "AbilityPickup_02_DoubleJump", position, new Color(0.72f, 0.95f, 1f, 1f));
            pickup.AddComponent<DoubleJumpAbilityPickup2D>().Configure(pickup.GetComponent<SpriteRenderer>());
        }

        private static GameObject CreatePickupBase(Sprite sprite, string name, Vector2 position, Color color)
        {
            GameObject pickup = new GameObject(name);
            pickup.transform.position = position;
            pickup.transform.localScale = new Vector3(0.44f, 0.44f, 1f);

            CircleCollider2D trigger = pickup.AddComponent<CircleCollider2D>();
            trigger.radius = 0.45f;
            trigger.isTrigger = true;

            SpriteRenderer visual = pickup.AddComponent<SpriteRenderer>();
            visual.sprite = sprite;
            visual.color = color;
            visual.sortingOrder = 18;
            return pickup;
        }

        private void CreateCheckpoint(Sprite sprite, string name, Vector2 position)
        {
            GameObject checkpoint = new GameObject(name);
            checkpoint.transform.position = position;

            BoxCollider2D trigger = checkpoint.AddComponent<BoxCollider2D>();
            trigger.size = new Vector2(1.3f, 0.85f);
            trigger.offset = new Vector2(0f, 0.55f);
            trigger.isTrigger = true;

            SpriteRenderer marker = CreateChildVisual(
                sprite,
                "Marker",
                checkpoint.transform,
                Vector2.zero,
                new Vector2(0.35f, 1.6f),
                new Color(0.25f, 0.45f, 0.42f, 0.75f),
                15);

            checkpoint.AddComponent<Checkpoint2D>().Configure(checkpoint.transform, marker);
        }

        private void CreateShard(Sprite sprite, string name, Vector2 position)
        {
            GameObject shard = new GameObject(name);
            shard.transform.position = position;
            shard.transform.localScale = new Vector3(0.32f, 0.32f, 1f);

            CircleCollider2D trigger = shard.AddComponent<CircleCollider2D>();
            trigger.radius = 0.4f;
            trigger.isTrigger = true;

            SpriteRenderer visual = shard.AddComponent<SpriteRenderer>();
            visual.sprite = sprite;
            visual.color = new Color(0.95f, 0.86f, 0.35f, 1f);
            visual.sortingOrder = 17;

            shard.AddComponent<ShanhaiShardCollectible2D>().Configure(visual);
        }

        private static void CreateBox(Sprite sprite, string name, Vector2 position, Vector2 size, Color color)
        {
            GameObject box = new GameObject(name);
            box.transform.position = position;
            box.transform.localScale = size;

            SpriteRenderer renderer = box.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = 0;

            BoxCollider2D collider = box.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
        }

        private static void CreateHazard(Sprite sprite, string name, Vector2 position, Vector2 size, Color color)
        {
            GameObject hazard = new GameObject(name);
            hazard.transform.position = position;
            hazard.transform.localScale = size;

            SpriteRenderer renderer = hazard.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = 9;

            BoxCollider2D trigger = hazard.AddComponent<BoxCollider2D>();
            trigger.size = Vector2.one;
            trigger.isTrigger = true;

            hazard.AddComponent<HazardRespawn2D>().Configure(name);
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
                    return new Color(0.24f, 0.29f, 0.31f);
                case RouteBand.Ability:
                    return new Color(0.17f, 0.36f, 0.34f);
                case RouteBand.AbilityGate:
                    return new Color(0.31f, 0.24f, 0.41f);
                case RouteBand.Return:
                    return new Color(0.17f, 0.25f, 0.38f);
                case RouteBand.Shard:
                    return new Color(0.42f, 0.36f, 0.16f);
                case RouteBand.Checkpoint:
                    return new Color(0.19f, 0.35f, 0.29f);
                case RouteBand.Bamboo:
                    return new Color(0.23f, 0.38f, 0.23f);
                case RouteBand.Mechanism:
                    return new Color(0.42f, 0.31f, 0.22f);
                case RouteBand.Goal:
                    return new Color(0.33f, 0.27f, 0.18f);
                default:
                    return new Color(0.2f, 0.3f, 0.28f);
            }
        }

        private static Color HazardColor(HazardBand band)
        {
            switch (band)
            {
                case HazardBand.Fire:
                    return new Color(1f, 0.36f, 0.12f, 0.72f);
                case HazardBand.Spike:
                    return new Color(0.9f, 0.12f, 0.18f, 0.72f);
                case HazardBand.Water:
                    return new Color(0.14f, 0.45f, 0.72f, 0.6f);
                case HazardBand.Mechanism:
                    return new Color(0.88f, 0.24f, 0.12f, 0.58f);
                default:
                    return new Color(0.65f, 0.12f, 0.18f, 0.6f);
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
            squareSprite.name = "Runtime_ShanhaiGateGrayboxSquare";
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
            AbilityGate,
            Return,
            Shard,
            Checkpoint,
            Bamboo,
            Mechanism,
            Goal
        }

        private enum HazardBand
        {
            Fire,
            Spike,
            Water,
            Mechanism,
            Fall
        }
    }
}
