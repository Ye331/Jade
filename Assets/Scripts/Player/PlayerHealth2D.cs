using UnityEngine;

namespace Jade.Player
{
    [RequireComponent(typeof(PlayerMotor2D))]
    public class PlayerHealth2D : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 3;

        private int currentHealth;
        private PlayerMotor2D motor;

        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;
        public event System.Action<int, int> HealthChanged;

        private void Awake()
        {
            motor = GetComponent<PlayerMotor2D>();
            currentHealth = Mathf.Max(1, maxHealth);
            NotifyHealthChanged();
        }

        public void TakeDamage(int amount, string source)
        {
            if (amount <= 0 || currentHealth <= 0)
            {
                return;
            }

            currentHealth = Mathf.Max(0, currentHealth - amount);
            Debug.Log("Player took " + amount + " damage from " + source + ". HP: " + currentHealth + "/" + maxHealth);
            NotifyHealthChanged();

            if (currentHealth <= 0)
            {
                Debug.Log("Player health depleted; respawning with full health.");
                if (motor != null)
                {
                    motor.RespawnAtSpawnPoint();
                }

                RestoreFullHealth();
            }
        }

        public void RestoreFullHealth()
        {
            currentHealth = Mathf.Max(1, maxHealth);
            Debug.Log("Player health restored to " + currentHealth + "/" + maxHealth + ".");
            NotifyHealthChanged();
        }

        private void NotifyHealthChanged()
        {
            if (HealthChanged != null)
            {
                HealthChanged(currentHealth, maxHealth);
            }
        }
    }
}
