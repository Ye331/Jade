using Jade.Player;
using UnityEngine;

namespace Jade.World
{
    public class MonsterLobber2D : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Transform firePoint;
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private float fireInterval = 2.2f;
        [SerializeField] private float projectileLifetime = 5f;
        [SerializeField] private float arcHeight = 3.2f;
        [SerializeField] private float projectileGravityScale = 1f;
        [SerializeField] private Color projectileColor = new Color(1f, 0.48f, 0.12f, 1f);
        [SerializeField] private float attackRangeX = 20f;

        private static Sprite fallbackProjectileSprite;
        private float fireTimer;

        public void Configure(Transform playerTarget, Transform muzzle, Sprite sprite)
        {
            target = playerTarget;
            firePoint = muzzle;
            projectileSprite = sprite;
        }

        private void Awake()
        {
            fireTimer = fireInterval * 0.45f;
        }

        private void Update()
        {
            if (target == null)
            {
                PlayerMotor2D player = FindObjectOfType<PlayerMotor2D>();
                if (player != null)
                {
                    target = player.transform;
                }
            }

            if (firePoint == null)
            {
                firePoint = transform;
            }

            if (target == null)
            {
                return;
            }

            if (projectileSprite == null)
            {
                projectileSprite = GetFallbackProjectileSprite();
            }

            if (Mathf.Abs(target.position.x - transform.position.x) > attackRangeX)
            {
                return;
            }

            fireTimer -= Time.deltaTime;
            if (fireTimer > 0f)
            {
                return;
            }

            fireTimer = fireInterval;
            FireAtTarget();
        }

        private void FireAtTarget()
        {
            Vector2 start = firePoint.position;
            Vector2 end = target.position;
            Vector2 velocity = CalculateLobVelocity(start, end);

            GameObject projectile = new GameObject("Monster_ArcProjectile");
            projectile.transform.position = start;
            projectile.transform.localScale = new Vector3(0.35f, 0.35f, 1f);

            SpriteRenderer renderer = projectile.AddComponent<SpriteRenderer>();
            renderer.sprite = projectileSprite;
            renderer.color = projectileColor;
            renderer.sortingOrder = 12;

            CircleCollider2D collider = projectile.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;
            collider.isTrigger = true;

            Rigidbody2D body = projectile.AddComponent<Rigidbody2D>();
            body.gravityScale = projectileGravityScale;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;

            MonsterProjectile2D projectileComponent = projectile.AddComponent<MonsterProjectile2D>();
            projectileComponent.Launch(velocity, projectileLifetime);
        }

        private Vector2 CalculateLobVelocity(Vector2 start, Vector2 end)
        {
            float gravity = Mathf.Abs(Physics2D.gravity.y * projectileGravityScale);
            if (gravity <= 0.01f)
            {
                gravity = 9.81f;
            }

            float apexY = Mathf.Max(start.y, end.y) + arcHeight;
            float upTime = Mathf.Sqrt(2f * Mathf.Max(0.1f, apexY - start.y) / gravity);
            float downTime = Mathf.Sqrt(2f * Mathf.Max(0.1f, apexY - end.y) / gravity);
            float totalTime = Mathf.Max(0.25f, upTime + downTime);

            float vx = (end.x - start.x) / totalTime;
            float vy = gravity * upTime;
            return new Vector2(vx, vy);
        }

        private static Sprite GetFallbackProjectileSprite()
        {
            if (fallbackProjectileSprite != null)
            {
                return fallbackProjectileSprite;
            }

            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.name = "Runtime_MonsterProjectileSquare";
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();

            fallbackProjectileSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            fallbackProjectileSprite.name = "Runtime_MonsterProjectileSquare";
            return fallbackProjectileSprite;
        }
    }
}
