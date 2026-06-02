using Jade.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Jade.World
{
    public class HazardRespawn2D : MonoBehaviour
    {
        [SerializeField] private string hazardName = "Hazard";
        [SerializeField] private int damage = 1;
        [SerializeField] private float damageCooldown = 1f;

        private readonly Dictionary<PlayerHealth2D, float> nextDamageTimes = new Dictionary<PlayerHealth2D, float>();

        public void Configure(string displayName)
        {
            hazardName = displayName;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryDamage(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryDamage(other);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            TryDamage(collision.collider);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            TryDamage(collision.collider);
        }

        private void TryDamage(Collider2D other)
        {
            PlayerHealth2D health = other.GetComponent<PlayerHealth2D>();
            if (health == null)
            {
                return;
            }

            float now = Time.time;
            if (nextDamageTimes.TryGetValue(health, out float nextDamageTime) && now < nextDamageTime)
            {
                return;
            }

            health.TakeDamage(damage, hazardName);
            nextDamageTimes[health] = now + damageCooldown;
        }
    }
}
