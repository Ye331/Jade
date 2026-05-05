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

            CreatePlayerVisuals(player, sprite);

            return player;
        }

        private void CreatePlayerVisuals(GameObject player, Sprite sprite)
        {
            GameObject visualRoot = new GameObject("VisualRoot_TemporaryCharacter");
            visualRoot.transform.SetParent(player.transform);
            visualRoot.transform.localPosition = Vector3.zero;

            CreateVisualPart(sprite, "Body_InkSilhouette", visualRoot.transform, new Vector2(0f, 0f), new Vector2(0.62f, 1.15f), new Color(0.08f, 0.12f, 0.13f), 12);
            CreateVisualPart(sprite, "Head_JadeGlow", visualRoot.transform, new Vector2(0.05f, 0.78f), new Vector2(0.42f, 0.38f), new Color(0.35f, 0.95f, 0.86f), 13);
            CreateVisualPart(sprite, "Tail_Ribbon", visualRoot.transform, new Vector2(-0.42f, 0.2f), new Vector2(0.55f, 0.14f), new Color(0.18f, 0.85f, 0.78f, 0.78f), 11);
            CreateVisualPart(sprite, "Foot_Shadow", visualRoot.transform, new Vector2(0f, -0.7f), new Vector2(0.82f, 0.12f), new Color(0.03f, 0.04f, 0.04f, 0.65f), 9);

            ParticleSystem landingDust = CreateDust("LandingDust", player.transform, new Vector3(0f, -0.78f, 0f), false);
            ParticleSystem runDust = CreateDust("RunDust", player.transform, new Vector3(-0.25f, -0.78f, 0f), true);
            TrailRenderer trail = CreateTrail(visualRoot.transform);

            PlayerVisualFeedback2D feedback = player.AddComponent<PlayerVisualFeedback2D>();
            feedback.Configure(visualRoot.transform, landingDust, runDust, trail);
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

        private static TrailRenderer CreateTrail(Transform parent)
        {
            GameObject trailObject = new GameObject("SpeedTrail_JadeRibbon");
            trailObject.transform.SetParent(parent);
            trailObject.transform.localPosition = new Vector3(-0.48f, 0.24f, 0f);

            TrailRenderer trail = trailObject.AddComponent<TrailRenderer>();
            trail.time = 0.16f;
            trail.startWidth = 0.16f;
            trail.endWidth = 0f;
            trail.emitting = false;
            trail.sortingOrder = 7;
            trail.material = new Material(Shader.Find("Sprites/Default"));
            trail.startColor = new Color(0.38f, 1f, 0.9f, 0.5f);
            trail.endColor = new Color(0.38f, 1f, 0.9f, 0f);

            return trail;
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
