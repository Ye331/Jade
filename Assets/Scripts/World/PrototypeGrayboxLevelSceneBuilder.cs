using Jade.CameraTools;
using Jade.Player;
using UnityEngine;

namespace Jade.World
{
    public class PrototypeGrayboxLevelSceneBuilder : MonoBehaviour
    {
        [SerializeField] private PlayerMovementSettings movementSettings;
        [SerializeField] private Camera sceneCamera;

        private static Sprite squareSprite;
        private static Sprite backgroundSprite;
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
            CreateRoute(sprite);
            CreateCheckpoint(sprite, "Checkpoint_MidRoute", new Vector2(24f, -0.05f));
            CreateCollectibles(sprite);
            CreateGoal(sprite, new Vector2(55f, 1.6f));
            CreateRespawnZone();
        }

        private GameObject CreatePlayer(Sprite sprite)
        {
            GameObject player = new GameObject("Player_GrayboxRoute");
            player.transform.position = new Vector3(-9f, -1.7f, 0f);

            Rigidbody2D body = player.AddComponent<Rigidbody2D>();
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;

            BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.68f, 1.25f);

            player.AddComponent<PlayerInputReader>();
            PlayerMotor2D motor = player.AddComponent<PlayerMotor2D>();
            motor.Configure(movementSettings);
            motor.SetSpawnPoint(player.transform.position);

            CreatePlayerVisuals(player, sprite);
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
            cameraToUse.backgroundColor = new Color(0.075f, 0.085f, 0.095f);
            cameraToUse.transform.position = new Vector3(-6f, 0f, -10f);

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
            CreateBox(sprite, "Start_LongRun", new Vector2(-5f, -3f), new Vector2(12f, 1f), PlatformColor(0));
            CreateBox(sprite, "GapJump_Short_01", new Vector2(4f, -2.2f), new Vector2(2.4f, 0.45f), PlatformColor(1));
            CreateBox(sprite, "GapJump_Short_02", new Vector2(8f, -1.25f), new Vector2(2.4f, 0.45f), PlatformColor(1));
            CreateBox(sprite, "MainRoute_BranchEntry", new Vector2(14f, -0.25f), new Vector2(4.5f, 0.45f), PlatformColor(2));

            CreateBox(sprite, "LowerCollectible_DropIn", new Vector2(12.5f, -5.45f), new Vector2(4.5f, 0.45f), PlatformColor(3));
            CreateBox(sprite, "LowerCollectible_Run", new Vector2(18.5f, -5.45f), new Vector2(4.5f, 0.45f), PlatformColor(3));
            CreateBox(sprite, "Recovery_FromLower_01", new Vector2(25.5f, -3.95f), new Vector2(2.4f, 0.45f), PlatformColor(3));
            CreateBox(sprite, "Recovery_FromLower_02", new Vector2(30f, -2.35f), new Vector2(2.4f, 0.45f), PlatformColor(3));

            CreateBox(sprite, "Checkpoint_Platform", new Vector2(24f, -1.1f), new Vector2(7f, 0.55f), PlatformColor(2));
            CreateBox(sprite, "ClimbStep_01", new Vector2(31f, -0.1f), new Vector2(2.4f, 0.45f), PlatformColor(4));
            CreateBox(sprite, "ClimbStep_02", new Vector2(35f, 0.95f), new Vector2(2.4f, 0.45f), PlatformColor(4));
            CreateBox(sprite, "ClimbStep_03", new Vector2(39f, 1.8f), new Vector2(2.4f, 0.45f), PlatformColor(4));
            CreateBox(sprite, "Final_Runup", new Vector2(46f, 1.35f), new Vector2(8f, 0.5f), PlatformColor(5));
            CreateBox(sprite, "Goal_Platform", new Vector2(55f, 1.35f), new Vector2(4f, 0.5f), PlatformColor(5));
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

        private void CreateCheckpoint(Sprite sprite, string name, Vector2 position)
        {
            GameObject checkpoint = new GameObject(name);
            checkpoint.transform.position = position;

            BoxCollider2D trigger = checkpoint.AddComponent<BoxCollider2D>();
            trigger.size = new Vector2(1.25f, 0.85f);
            trigger.offset = new Vector2(0f, 0.55f);
            trigger.isTrigger = true;

            SpriteRenderer marker = CreateChildVisual(sprite, "Marker", checkpoint.transform, Vector2.zero, new Vector2(0.35f, 1.6f), new Color(0.25f, 0.45f, 0.42f, 0.75f), 15);
            Checkpoint2D checkpointComponent = checkpoint.AddComponent<Checkpoint2D>();
            checkpointComponent.Configure(checkpoint.transform, marker);
        }

        private void CreateCollectibles(Sprite sprite)
        {
            CreateCollectible(sprite, "Collectible_LowerUnderCheckpoint", new Vector2(25.5f, -3.15f));
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
            GameObject goal = new GameObject("LevelGoal_EndMarker");
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
            GameObject zone = new GameObject("RespawnZone_GrayboxFallReset");
            zone.transform.position = new Vector3(23f, -10.5f, 0f);

            BoxCollider2D collider = zone.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(95f, 1f);
            collider.isTrigger = true;

            zone.AddComponent<RespawnZone2D>();
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
                animator.Configure(characterRenderer, LoadCharacterFrameTextures(), 250f);
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

        private static SpriteRenderer CreateAnimatedCharacterVisual(Transform visualRoot)
        {
            Texture2D[] frameTextures = LoadCharacterFrameTextures();
            if (frameTextures == null)
            {
                return null;
            }

            GameObject spriteObject = new GameObject("JadeQilin_FrameSprite");
            spriteObject.transform.SetParent(visualRoot);
            spriteObject.transform.localPosition = new Vector3(0f, -0.68f, 0f);
            spriteObject.transform.localScale = Vector3.one;

            SpriteRenderer renderer = spriteObject.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = 16;
            return renderer;
        }

        private static Texture2D[] LoadCharacterFrameTextures()
        {
            Texture2D[] textures = new Texture2D[CharacterFrameNames.Length];
            for (int i = 0; i < CharacterFrameNames.Length; i++)
            {
                textures[i] = Resources.Load<Texture2D>("Characters/JadeQilinFrames/JadeQilin_" + CharacterFrameNames[i]);
                if (textures[i] == null)
                {
                    return null;
                }
            }

            return textures;
        }

        private static void CreateSpiritAnimalProxy(Sprite sprite, Transform visualRoot)
        {
            Color ink = new Color(0.12f, 0.24f, 0.23f);
            Color deepInk = new Color(0.06f, 0.12f, 0.13f);
            Color jade = new Color(0.4f, 1f, 0.86f, 0.95f);
            Color paleJade = new Color(0.72f, 1f, 0.92f, 0.95f);

            CreateChildVisual(sprite, "Tail_Ribbon_Long", visualRoot, new Vector2(-0.58f, 0.12f), new Vector2(0.86f, 0.14f), new Color(0.42f, 1f, 0.9f, 0.62f), 10);
            CreateChildVisual(sprite, "Tail_Ribbon_Tip", visualRoot, new Vector2(-0.94f, 0.28f), new Vector2(0.34f, 0.1f), new Color(0.62f, 1f, 0.94f, 0.65f), 10);
            CreateChildVisual(sprite, "Tail_Ink_Base", visualRoot, new Vector2(-0.32f, 0.04f), new Vector2(0.36f, 0.1f), deepInk, 11);
            CreateChildVisual(sprite, "Body_JadeInk", visualRoot, new Vector2(0f, -0.12f), new Vector2(0.68f, 0.46f), ink, 12);
            CreateChildVisual(sprite, "Belly_Mist", visualRoot, new Vector2(0.08f, -0.2f), new Vector2(0.44f, 0.18f), new Color(0.56f, 0.9f, 0.78f, 0.55f), 13);
            CreateChildVisual(sprite, "Chest_JadeCore", visualRoot, new Vector2(0.26f, 0.02f), new Vector2(0.15f, 0.22f), jade, 15);
            CreateChildVisual(sprite, "Head_JadeInk", visualRoot, new Vector2(0.39f, 0.31f), new Vector2(0.4f, 0.32f), ink, 13);
            CreateChildVisual(sprite, "Muzzle_Glow", visualRoot, new Vector2(0.62f, 0.25f), new Vector2(0.22f, 0.14f), new Color(0.5f, 0.95f, 0.82f, 0.72f), 14);
            CreateChildVisual(sprite, "Eye_Jade", visualRoot, new Vector2(0.53f, 0.38f), new Vector2(0.08f, 0.07f), paleJade, 16);
            CreateChildVisual(sprite, "Ear_Back", visualRoot, new Vector2(0.22f, 0.58f), new Vector2(0.12f, 0.32f), new Color(0.16f, 0.36f, 0.32f), 12);
            CreateChildVisual(sprite, "Ear_Front_Glow", visualRoot, new Vector2(0.43f, 0.58f), new Vector2(0.1f, 0.3f), new Color(0.48f, 0.98f, 0.84f, 0.9f), 15);
            CreateChildVisual(sprite, "Horn_Jade_A", visualRoot, new Vector2(0.35f, 0.76f), new Vector2(0.07f, 0.24f), paleJade, 16);
            CreateChildVisual(sprite, "Horn_Jade_B", visualRoot, new Vector2(0.49f, 0.73f), new Vector2(0.06f, 0.2f), paleJade, 16);
            CreateChildVisual(sprite, "Leg_Front", visualRoot, new Vector2(0.24f, -0.46f), new Vector2(0.1f, 0.34f), deepInk, 12);
            CreateChildVisual(sprite, "Leg_Back", visualRoot, new Vector2(-0.24f, -0.46f), new Vector2(0.1f, 0.34f), deepInk, 12);
            CreateChildVisual(sprite, "Hoof_Front_Glow", visualRoot, new Vector2(0.24f, -0.66f), new Vector2(0.14f, 0.06f), jade, 15);
            CreateChildVisual(sprite, "Hoof_Back_Glow", visualRoot, new Vector2(-0.24f, -0.66f), new Vector2(0.14f, 0.06f), jade, 15);
            CreateChildVisual(sprite, "Foot_Shadow", visualRoot, new Vector2(0f, -0.72f), new Vector2(0.78f, 0.06f), new Color(0.03f, 0.04f, 0.04f, 0.48f), 9);
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

        private static Color PlatformColor(int index)
        {
            Color[] colors =
            {
                new Color(0.18f, 0.22f, 0.24f),
                new Color(0.23f, 0.28f, 0.3f),
                new Color(0.2f, 0.3f, 0.28f),
                new Color(0.16f, 0.2f, 0.24f),
                new Color(0.24f, 0.25f, 0.22f),
                new Color(0.3f, 0.25f, 0.2f)
            };

            return colors[Mathf.Clamp(index, 0, colors.Length - 1)];
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
