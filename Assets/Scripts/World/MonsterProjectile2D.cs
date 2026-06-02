using Jade.Player;
using UnityEngine;

namespace Jade.World
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class MonsterProjectile2D : MonoBehaviour
    {
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private string projectileName = "Monster Projectile";

        private Rigidbody2D body;
        private float timer;

        public void Launch(Vector2 velocity, float projectileLifetime)
        {
            lifetime = projectileLifetime;
            timer = 0f;
            if (body == null)
            {
                body = GetComponent<Rigidbody2D>();
            }

            body.velocity = velocity;
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;

            Collider2D projectileCollider = GetComponent<Collider2D>();
            projectileCollider.isTrigger = true;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= lifetime)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            BreakablePlatform2D breakable = other.GetComponent<BreakablePlatform2D>();
            if (breakable != null)
            {
                breakable.Break();
                Destroy(gameObject);
                return;
            }

            PlayerHealth2D health = other.GetComponent<PlayerHealth2D>();
            if (health != null)
            {
                health.TakeDamage(1, projectileName);
                Destroy(gameObject);
                return;
            }

            if (!other.isTrigger)
            {
                Destroy(gameObject);
            }
        }
    }
}
