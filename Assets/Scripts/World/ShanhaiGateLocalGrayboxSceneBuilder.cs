using Jade.CameraTools;
using Jade.Player;
using UnityEngine;

namespace Jade.World
{
    public class ShanhaiGateLocalGrayboxSceneBuilder : MonoBehaviour
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
            CreateRoute(sprite);
            CreateRespawnZone();
        }

        private GameObject CreatePlayer()
        {
            Vector3 spawn = new Vector3(-11.5f, -1.65f, 0f);
            GameObject prefab = Resources.Load<GameObject>(PlayerPrefabResourcePath);
            if (prefab == null)
            {
                prefab = Resources.Load<GameObject>(LegacyPlayerPrefabResourcePath);
            }

            GameObject player;
            if (prefab != null)
            {
                player = Instantiate(prefab, spawn, Quaternion.identity);
                player.name = "Player_ShanhaiGate_LocalGraybox";
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
            cameraToUse.orthographicSize = 5.6f;
            cameraToUse.backgroundColor = new Color(0.06f, 0.08f, 0.085f);
            cameraToUse.transform.position = new Vector3(-8f, -0.5f, -10f);

            SimpleCameraFollow2D follow = cameraToUse.GetComponent<SimpleCameraFollow2D>();
            if (follow == null)
            {
                follow = cameraToUse.gameObject.AddComponent<SimpleCameraFollow2D>();
            }

            follow.Configure(target);
        }

        private void CreateRoute(Sprite sprite)
        {
            CreateBox(sprite, "Start_ContinuousLand_12m", new Vector2(-6f, -3.5f), new Vector2(14f, 1f), LandColor());
            CreateHazard(sprite, "Gap01_FallCatch", new Vector2(1.85f, -4.15f), new Vector2(1.7f, 0.35f), FallHazardColor());
            CreateBox(sprite, "MainLand_AfterShortGap", new Vector2(6.35f, -3.5f), new Vector2(7.3f, 1f), LandColor());
            CreateHazard(sprite, "SpikeRead_ShallowTrap", new Vector2(7.25f, -2.68f), new Vector2(1.65f, 0.45f), SpikeHazardColor());
            CreateBox(sprite, "LowStep_Land", new Vector2(12.5f, -2.85f), new Vector2(5f, 1f), StepColor());
            CreateHazard(sprite, "Gap02_RunJumpFallCatch", new Vector2(16.5f, -3.85f), new Vector2(3f, 0.5f), FallHazardColor());
            CreateBox(sprite, "MechanicIntro_LandBeforeDropHatch", new Vector2(23f, -2.85f), new Vector2(10f, 1f), GoalColor());
            CreateBreakablePlatform(sprite, "MonsterShot_BreakableDropHatch", new Vector2(29.2f, -2.85f), new Vector2(3.3f, 0.65f));
            CreateBox(sprite, "Hatch_RightLip_PlayerBaitSpot", new Vector2(32.5f, -2.85f), new Vector2(3f, 1f), LandColor());
            CreateBox(sprite, "Monster_LobberPerch", new Vector2(38.5f, -1.8f), new Vector2(3f, 1f), StepColor());
            CreateMonsterLobber(sprite, new Vector2(38.5f, -0.85f), new Vector2(37.15f, -0.45f));

            CreateBox(sprite, "LowerRoute_DropCatch", new Vector2(29.2f, -6.3f), new Vector2(5.5f, 1f), StepColor());
            CreateBox(sprite, "LowerRoute_Runout", new Vector2(36.8f, -6.3f), new Vector2(8f, 1f), LandColor());
            CreateHazard(sprite, "LowerRoute_SmallPitCatch", new Vector2(33.1f, -7.55f), new Vector2(2.2f, 0.45f), FallHazardColor());
            CreateBox(sprite, "Goal_ContinuousLand_Stop", new Vector2(44f, -6.3f), new Vector2(6f, 1f), GoalColor());
            CreateGoal(sprite, new Vector2(46.2f, -5.3f));
        }

        private static void CreateBox(Sprite sprite, string name, Vector2 position, Vector2 size, Color color)
        {
            GameObject box = new GameObject(name);
            box.transform.position = new Vector3(position.x, position.y, 0f);
            box.transform.localScale = new Vector3(size.x, size.y, 1f);

            SpriteRenderer renderer = box.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = 2;

            BoxCollider2D collider = box.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
        }

        private static void CreateHazard(Sprite sprite, string name, Vector2 position, Vector2 size, Color color)
        {
            GameObject hazard = new GameObject(name);
            hazard.transform.position = new Vector3(position.x, position.y, 0f);
            hazard.transform.localScale = new Vector3(size.x, size.y, 1f);

            SpriteRenderer renderer = hazard.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = 5;

            BoxCollider2D collider = hazard.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
            collider.isTrigger = true;

            hazard.AddComponent<HazardRespawn2D>().Configure(name);
        }

        private static void CreateBreakablePlatform(Sprite sprite, string name, Vector2 position, Vector2 size)
        {
            GameObject platform = new GameObject(name);
            platform.transform.position = new Vector3(position.x, position.y, 0f);
            platform.transform.localScale = new Vector3(size.x, size.y, 1f);

            SpriteRenderer renderer = platform.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = BreakableColor();
            renderer.sortingOrder = 4;

            BoxCollider2D collider = platform.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;

            platform.AddComponent<BreakablePlatform2D>().Configure(name, renderer, collider);

            CreateChildVisual(
                sprite,
                "CrackMarker_Left",
                platform.transform,
                new Vector2(-0.18f, 0f),
                new Vector2(0.035f, 1.1f),
                new Color(0.35f, 0.16f, 0.08f, 0.7f),
                5,
                -18f);
            CreateChildVisual(
                sprite,
                "CrackMarker_Right",
                platform.transform,
                new Vector2(0.18f, 0f),
                new Vector2(0.035f, 1.05f),
                new Color(0.35f, 0.16f, 0.08f, 0.7f),
                5,
                22f);
        }

        private static void CreateMonsterLobber(Sprite sprite, Vector2 position, Vector2 firePointPosition)
        {
            GameObject monster = new GameObject("Monster_Lobber_RightPerch");
            monster.transform.position = new Vector3(position.x, position.y, 0f);
            monster.transform.localScale = new Vector3(0.9f, 1.4f, 1f);

            SpriteRenderer renderer = monster.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = MonsterColor();
            renderer.sortingOrder = 8;

            BoxCollider2D collider = monster.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
            collider.isTrigger = true;

            GameObject firePoint = new GameObject("FirePoint");
            firePoint.transform.SetParent(monster.transform);
            firePoint.transform.position = new Vector3(firePointPosition.x, firePointPosition.y, 0f);

            SpriteRenderer eye = CreateChildVisual(
                sprite,
                "LobberEye",
                monster.transform,
                new Vector2(-0.18f, 0.18f),
                new Vector2(0.22f, 0.16f),
                new Color(1f, 0.48f, 0.18f, 1f),
                9,
                0f);

            MonsterLobber2D lobber = monster.AddComponent<MonsterLobber2D>();
            lobber.Configure(null, firePoint.transform, sprite);
            eye.name = "LobberEye";
        }

        private static void CreateGoal(Sprite sprite, Vector2 position)
        {
            GameObject goal = new GameObject("Goal_TestEndMarker");
            goal.transform.position = new Vector3(position.x, position.y, 0f);

            BoxCollider2D trigger = goal.AddComponent<BoxCollider2D>();
            trigger.size = new Vector2(1.2f, 2.5f);
            trigger.isTrigger = true;

            SpriteRenderer renderer = goal.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(0.3f, 0.95f, 0.78f, 0.85f);
            renderer.sortingOrder = 6;
            goal.transform.localScale = new Vector3(0.45f, 2.2f, 1f);

            goal.AddComponent<LevelGoal2D>().Configure(renderer);
        }

        private static SpriteRenderer CreateChildVisual(
            Sprite sprite,
            string name,
            Transform parent,
            Vector2 localPosition,
            Vector2 localScale,
            Color color,
            int sortingOrder,
            float rotationZ)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(parent);
            child.transform.localPosition = new Vector3(localPosition.x, localPosition.y, 0f);
            child.transform.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
            child.transform.localScale = new Vector3(localScale.x, localScale.y, 1f);

            SpriteRenderer renderer = child.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        private void CreateRespawnZone()
        {
            GameObject zone = new GameObject("RespawnZone_ShanhaiGateLocalFallReset");
            zone.transform.position = new Vector3(16f, -10.5f, 0f);

            BoxCollider2D collider = zone.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(76f, 2.5f);
            collider.isTrigger = true;

            zone.AddComponent<RespawnZone2D>();
        }

        private static Sprite GetSquareSprite()
        {
            if (squareSprite != null)
            {
                return squareSprite;
            }

            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.name = "Runtime_ShanhaiGateLocalSquare";
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();

            squareSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            squareSprite.name = "Runtime_ShanhaiGateLocalSquare";
            return squareSprite;
        }

        private static Color LandColor()
        {
            return new Color(0.18f, 0.24f, 0.25f, 1f);
        }

        private static Color StepColor()
        {
            return new Color(0.2f, 0.3f, 0.29f, 1f);
        }

        private static Color GoalColor()
        {
            return new Color(0.22f, 0.34f, 0.32f, 1f);
        }

        private static Color BreakableColor()
        {
            return new Color(0.88f, 0.66f, 0.25f, 1f);
        }

        private static Color MonsterColor()
        {
            return new Color(0.36f, 0.12f, 0.32f, 1f);
        }

        private static Color SpikeHazardColor()
        {
            return new Color(0.95f, 0.33f, 0.18f, 0.9f);
        }

        private static Color FallHazardColor()
        {
            return new Color(0.9f, 0.22f, 0.12f, 0.45f);
        }
    }
}
