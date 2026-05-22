using UnityEngine;

namespace Jade.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(PlayerAbilityInventory2D))]
    public class PlayerMotor2D : MonoBehaviour
    {
        [SerializeField] private PlayerMovementSettings settings;

        private Rigidbody2D body;
        private Collider2D bodyCollider;
        private PlayerInputReader input;
        private PlayerAbilityInventory2D abilities;
        private Vector3 spawnPosition;

        private float coyoteCounter;
        private float jumpBufferCounter;
        private float dashTimer;
        private float dashCooldownCounter;
        private float gravity;
        private float jumpVelocity;
        private bool isGrounded;
        private bool wasGrounded;
        private bool landedThisFrame;
        private bool jumpedThisFrame;
        private bool dashedThisFrame;
        private bool isDashing;
        private int airDashesRemaining;
        private int facingDirection = 1;
        private int dashDirection = 1;

        public int FacingDirection => facingDirection;
        public bool IsGrounded => isGrounded;
        public bool LandedThisFrame => landedThisFrame;
        public bool JumpedThisFrame => jumpedThisFrame;
        public bool DashedThisFrame => dashedThisFrame;
        public bool IsDashing => isDashing;
        public Vector2 Velocity => body != null ? body.velocity : Vector2.zero;
        public float VerticalSpeed => body != null ? body.velocity.y : 0f;
        public float Speed01 => settings != null && settings.maxRunSpeed > 0f
            ? Mathf.Clamp01(Mathf.Abs(Velocity.x) / settings.maxRunSpeed)
            : 0f;

        public void Configure(PlayerMovementSettings movementSettings)
        {
            settings = movementSettings;
            RecalculateJumpValues();
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            bodyCollider = GetComponent<Collider2D>();
            input = GetComponent<PlayerInputReader>();
            abilities = GetComponent<PlayerAbilityInventory2D>();
            if (abilities == null)
            {
                abilities = gameObject.AddComponent<PlayerAbilityInventory2D>();
            }
            spawnPosition = transform.position;

            body.gravityScale = 0f;
            body.freezeRotation = true;
            RecalculateJumpValues();
            ResetAirDashCount();
        }

        private void OnValidate()
        {
            RecalculateJumpValues();
        }

        private void FixedUpdate()
        {
            if (settings == null)
            {
                return;
            }

            landedThisFrame = false;
            jumpedThisFrame = false;
            dashedThisFrame = false;
            wasGrounded = isGrounded;

            UpdateGroundedState();
            UpdateTimers();
            TryStartDash();
            if (isDashing)
            {
                HandleDash();
                return;
            }

            HandleJumpInput();
            HandleRun();
            HandleGravity();
        }

        public void RespawnAtSpawnPoint()
        {
            body.velocity = Vector2.zero;
            transform.position = spawnPosition;
            isDashing = false;
            dashTimer = 0f;
            ResetAirDashCount();
        }

        public void SetSpawnPoint(Vector3 position)
        {
            spawnPosition = position;
        }

        private void RecalculateJumpValues()
        {
            if (settings == null || settings.timeToJumpApex <= 0f)
            {
                return;
            }

            gravity = -(2f * settings.jumpHeight) / (settings.timeToJumpApex * settings.timeToJumpApex);
            jumpVelocity = Mathf.Abs(gravity) * settings.timeToJumpApex;
        }

        private void UpdateGroundedState()
        {
            Bounds bounds = bodyCollider.bounds;
            Vector2 checkSize = new Vector2(bounds.size.x * settings.groundCheckWidth, bounds.size.y);
            RaycastHit2D[] hits = Physics2D.BoxCastAll(
                bounds.center,
                checkSize,
                0f,
                Vector2.down,
                settings.groundCheckDistance,
                settings.groundLayer);

            bool foundGround = false;
            for (int i = 0; i < hits.Length; i++)
            {
                Collider2D hitCollider = hits[i].collider;
                if (hitCollider != null && hitCollider != bodyCollider && !hitCollider.isTrigger)
                {
                    foundGround = true;
                    break;
                }
            }

            isGrounded = foundGround && body.velocity.y <= 0.05f;
            landedThisFrame = isGrounded && !wasGrounded;

            if (isGrounded)
            {
                coyoteCounter = settings.coyoteTime;
                ResetAirDashCount();
            }
        }

        private void UpdateTimers()
        {
            float step = Time.fixedDeltaTime;

            if (!isGrounded)
            {
                coyoteCounter -= step;
            }

            if (jumpBufferCounter > 0f)
            {
                jumpBufferCounter -= step;
            }

            if (dashCooldownCounter > 0f)
            {
                dashCooldownCounter -= step;
            }

            if (input.ConsumeJumpPressed())
            {
                jumpBufferCounter = settings.jumpBufferTime;
            }
        }

        private void TryStartDash()
        {
            if (!input.ConsumeDashPressed())
            {
                return;
            }

            if (abilities == null || !abilities.DashUnlocked || isGrounded || isDashing || airDashesRemaining <= 0 || dashCooldownCounter > 0f)
            {
                return;
            }

            float horizontal = input.Horizontal;
            dashDirection = Mathf.Abs(horizontal) > 0.01f ? (horizontal > 0f ? 1 : -1) : facingDirection;
            facingDirection = dashDirection;
            airDashesRemaining--;
            dashTimer = Mathf.Max(settings.dashDuration, Time.fixedDeltaTime);
            dashCooldownCounter = settings.dashCooldown;
            isDashing = true;
            dashedThisFrame = true;
            jumpBufferCounter = 0f;

            body.velocity = new Vector2(dashDirection * settings.dashSpeed, settings.dashVerticalSpeed);
        }

        private void HandleDash()
        {
            dashTimer -= Time.fixedDeltaTime;
            body.velocity = new Vector2(dashDirection * settings.dashSpeed, settings.dashVerticalSpeed);

            if (dashTimer <= 0f || isGrounded)
            {
                isDashing = false;
            }
        }

        private void ResetAirDashCount()
        {
            airDashesRemaining = settings != null ? Mathf.Max(0, settings.airDashCount) : 0;
        }

        private void HandleJumpInput()
        {
            if (jumpBufferCounter > 0f && coyoteCounter > 0f)
            {
                Vector2 velocity = body.velocity;
                velocity.y = jumpVelocity;
                body.velocity = velocity;

                jumpBufferCounter = 0f;
                coyoteCounter = 0f;
                isGrounded = false;
                jumpedThisFrame = true;
            }

            if (input.ConsumeJumpReleased() && body.velocity.y > 0f)
            {
                Vector2 velocity = body.velocity;
                velocity.y *= settings.jumpCutVelocityMultiplier;
                body.velocity = velocity;
            }
        }

        private void HandleRun()
        {
            float horizontal = input.Horizontal;

            if (Mathf.Abs(horizontal) > 0.01f)
            {
                int requestedFacing = horizontal > 0f ? 1 : -1;
                facingDirection = requestedFacing;
            }

            float targetSpeed = horizontal * settings.maxRunSpeed;
            float currentSpeed = body.velocity.x;
            bool turning = Mathf.Abs(horizontal) > 0.01f && Mathf.Sign(horizontal) != Mathf.Sign(currentSpeed) && Mathf.Abs(currentSpeed) > 0.05f;

            float rate;
            if (isGrounded)
            {
                rate = Mathf.Abs(horizontal) > 0.01f
                    ? (turning ? settings.turnDeceleration : settings.groundAcceleration)
                    : settings.groundDeceleration;
            }
            else
            {
                rate = Mathf.Abs(horizontal) > 0.01f ? settings.airAcceleration : settings.airDeceleration;
            }

            float newX = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * Time.fixedDeltaTime);
            body.velocity = new Vector2(newX, body.velocity.y);
        }

        private void HandleGravity()
        {
            Vector2 velocity = body.velocity;

            if (!isGrounded)
            {
                float multiplier = 1f;
                if (velocity.y < 0f)
                {
                    multiplier = settings.fallGravityMultiplier;
                }
                else if (velocity.y > 0f && !input.JumpHeld)
                {
                    multiplier = settings.lowJumpGravityMultiplier;
                }

                velocity.y += gravity * multiplier * Time.fixedDeltaTime;
                velocity.y = Mathf.Max(velocity.y, -settings.maxFallSpeed);
            }
            else if (velocity.y < 0f)
            {
                velocity.y = -0.5f;
            }

            body.velocity = velocity;
        }
    }
}
