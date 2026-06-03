using System.Collections;
using UnityEngine;

namespace Jade.Player
{
    [RequireComponent(typeof(PlayerMotor2D))]
    public class PlayerHealth2D : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 3;
        [Header("Damage Feedback")]
        [SerializeField] private Color damageFlashColor = new Color(1f, 0.22f, 0.18f, 1f);
        [SerializeField] private float damageFlashDuration = 0.55f;
        [SerializeField] private float damageFlashInterval = 0.08f;

        private int currentHealth;
        private PlayerMotor2D motor;
        private SpriteRenderer[] spriteRenderers;
        private Color[] originalSpriteColors;
        private Coroutine damageFlashRoutine;

        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;
        public event System.Action<int, int> HealthChanged;

        private void Awake()
        {
            motor = GetComponent<PlayerMotor2D>();
            currentHealth = Mathf.Max(1, maxHealth);
            CacheSpriteRenderers();
            NotifyHealthChanged();
        }

        private void OnDisable()
        {
            RestoreSpriteColors();
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
            PlayDamageFlash();

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

        private void PlayDamageFlash()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            if (damageFlashRoutine != null)
            {
                StopCoroutine(damageFlashRoutine);
                RestoreSpriteColors();
            }

            damageFlashRoutine = StartCoroutine(DamageFlashRoutine());
        }

        private IEnumerator DamageFlashRoutine()
        {
            CacheSpriteRenderers();

            float elapsed = 0f;
            bool useFlashColor = true;
            while (elapsed < damageFlashDuration)
            {
                SetSpriteFlash(useFlashColor);
                useFlashColor = !useFlashColor;

                float wait = Mathf.Max(0.02f, damageFlashInterval);
                elapsed += wait;
                yield return new WaitForSeconds(wait);
            }

            RestoreSpriteColors();
            damageFlashRoutine = null;
        }

        private void CacheSpriteRenderers()
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            originalSpriteColors = new Color[spriteRenderers.Length];
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                originalSpriteColors[i] = spriteRenderers[i] != null ? spriteRenderers[i].color : Color.white;
            }
        }

        private void SetSpriteFlash(bool useFlashColor)
        {
            if (spriteRenderers == null || originalSpriteColors == null)
            {
                return;
            }

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                SpriteRenderer spriteRenderer = spriteRenderers[i];
                if (spriteRenderer == null)
                {
                    continue;
                }

                Color baseColor = i < originalSpriteColors.Length ? originalSpriteColors[i] : Color.white;
                Color flashColor = damageFlashColor;
                flashColor.a = baseColor.a;
                spriteRenderer.color = useFlashColor ? flashColor : baseColor;
            }
        }

        private void RestoreSpriteColors()
        {
            if (spriteRenderers == null || originalSpriteColors == null)
            {
                return;
            }

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] != null && i < originalSpriteColors.Length)
                {
                    spriteRenderers[i].color = originalSpriteColors[i];
                }
            }
        }
    }
}
