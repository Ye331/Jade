using Jade.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Jade.World
{
    [RequireComponent(typeof(Collider2D))]
    public class WideFallingStoneTrap2D : MonoBehaviour
    {
        [SerializeField] private string hazardName = "Wide Falling Stone Trap";
        [SerializeField] private int damage = 1;
        [SerializeField] private float damageCooldown = 0.75f;
        [SerializeField] private Vector2 triggerAreaSize = new Vector2(4.5f, 2.5f);
        [SerializeField] private Vector2 triggerAreaOffset = new Vector2(0f, -2f);
        [SerializeField] private LayerMask playerLayers = ~0;
        [SerializeField] private float triggerDelay = 0.3f;
        [SerializeField] private float warningShake = 0.06f;
        [SerializeField] private float warningShakeFrequency = 35f;
        [SerializeField] private float fallDistance = 4f;
        [SerializeField] private float fallSpeed = 18f;
        [SerializeField] private float downHoldTime = 0.8f;
        [SerializeField] private float resetSpeed = 7f;
        [SerializeField] private bool autoSetColliderTrigger = true;

        private readonly Dictionary<PlayerHealth2D, float> nextDamageTimes = new Dictionary<PlayerHealth2D, float>();
        private Collider2D stoneCollider;
        private Vector3 startPosition;
        private float stateTimer;
        private TrapState state;

        private enum TrapState
        {
            Armed,
            Warning,
            Dropping,
            Down,
            Resetting
        }

        public void Configure(string displayName)
        {
            hazardName = displayName;
        }

        private void Awake()
        {
            stoneCollider = GetComponent<Collider2D>();
            if (autoSetColliderTrigger && stoneCollider != null)
            {
                stoneCollider.isTrigger = true;
            }

            startPosition = transform.position;
        }

        private void OnEnable()
        {
            state = TrapState.Armed;
            stateTimer = 0f;
            nextDamageTimes.Clear();
            transform.position = startPosition;
        }

        private void Update()
        {
            switch (state)
            {
                case TrapState.Armed:
                    if (PlayerInTriggerArea())
                    {
                        state = TrapState.Warning;
                        stateTimer = Mathf.Max(0f, triggerDelay);
                    }
                    break;

                case TrapState.Warning:
                    stateTimer -= Time.deltaTime;
                    ApplyWarningShake();
                    if (stateTimer <= 0f)
                    {
                        state = TrapState.Dropping;
                    }
                    break;

                case TrapState.Dropping:
                    MoveToward(DroppedPosition(), fallSpeed);
                    if (Reached(DroppedPosition()))
                    {
                        state = TrapState.Down;
                        stateTimer = Mathf.Max(0f, downHoldTime);
                    }
                    break;

                case TrapState.Down:
                    stateTimer -= Time.deltaTime;
                    if (stateTimer <= 0f)
                    {
                        state = TrapState.Resetting;
                    }
                    break;

                case TrapState.Resetting:
                    MoveToward(startPosition, resetSpeed);
                    if (Reached(startPosition))
                    {
                        transform.position = startPosition;
                        state = TrapState.Armed;
                    }
                    break;
            }
        }

        private bool PlayerInTriggerArea()
        {
            Vector2 center = (Vector2)startPosition + triggerAreaOffset;
            Collider2D[] hits = Physics2D.OverlapBoxAll(center, triggerAreaSize, 0f, playerLayers);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] != null && hits[i].GetComponent<PlayerMotor2D>() != null)
                {
                    return true;
                }
            }

            return false;
        }

        private void ApplyWarningShake()
        {
            float shake = Mathf.Max(0f, warningShake);
            float phase = Time.time * Mathf.Max(1f, warningShakeFrequency);
            Vector3 offset = new Vector3(
                Mathf.Sin(phase) * shake,
                Mathf.Cos(phase * 0.7f) * shake * 0.5f,
                0f);
            transform.position = startPosition + offset;
        }

        private Vector3 DroppedPosition()
        {
            return startPosition + Vector3.down * Mathf.Max(0f, fallDistance);
        }

        private void MoveToward(Vector3 target, float speed)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                Mathf.Max(0.01f, speed) * Time.deltaTime);
        }

        private bool Reached(Vector3 target)
        {
            return Vector3.Distance(transform.position, target) <= 0.01f;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryDamage(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryDamage(other);
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

        private void OnDrawGizmosSelected()
        {
            Vector3 origin = Application.isPlaying ? startPosition : transform.position;

            Gizmos.color = new Color(1f, 0.24f, 0.1f, 0.28f);
            Gizmos.DrawCube(origin + (Vector3)triggerAreaOffset, triggerAreaSize);

            Gizmos.color = new Color(1f, 0.08f, 0.04f, 0.85f);
            Gizmos.DrawLine(origin, origin + Vector3.down * Mathf.Max(0f, fallDistance));
        }
    }
}
