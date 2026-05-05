using UnityEngine;

namespace Jade.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(PlayerInputReader))]
    public class PlayerMotor2D : MonoBehaviour
    {
        [SerializeField] private PlayerMovementSettings settings;

        private Rigidbody2D body;
        private Collider2D bodyCollider;
        private PlayerInputReader input;
        private Vector3 spawnPosition;

        private float coyoteCounter;
        private float jumpBufferCounter;
        private float gravity;
        private float jumpVelocity;
        private bool isGrounded;
        private bool wasGrounded;
        private bool landedThisFrame;
        private bool jumpedThisFrame;
        private int facingDirection = 1;

        public int FacingDirection => facingDirection;
        public bool IsGrounded => isGrounded;
        public bool LandedThisFrame => landedThisFrame;
        public bool JumpedThisFrame => jumpedThisFrame;
        public Vector2 Velocity => body != null ? body.velocity : Vector2.zero;

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
            spawnPosition = transform.position;

            body.gravityScale = 0f;
            body.freezeRotation = true;
            RecalculateJumpValues();
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
            wasGrounded = isGrounded;

            UpdateGroundedState();
            UpdateTimers();
            HandleJumpInput();
            HandleRun();
            HandleGravity();
        }

        public void RespawnAtSpawnPoint()
        {
            body.velocity = Vector2.zero;
            transform.position = spawnPosition;
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

            if (input.ConsumeJumpPressed())
            {
                jumpBufferCounter = settings.jumpBufferTime;
            }
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
                facingDirection = horizontal > 0f ? 1 : -1;
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
