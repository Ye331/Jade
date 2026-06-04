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
        private float wallJumpControlLockCounter;
        private float gravity;
        private float jumpVelocity;
        private bool isGrounded;
        private bool wasGrounded;
        private bool isTouchingWall;
        private bool landedThisFrame;
        private bool jumpedThisFrame;
        private bool doubleJumpedThisFrame;
        private bool wallJumpedThisFrame;
        private bool dashedThisFrame;
        private bool isDashing;
        private int airJumpsRemaining;
        private int airDashesRemaining;
        private int facingDirection = 1;
        private int dashDirection = 1;
        private int wallDirection;

        [Header("=== 音效配置 ===")]
        //走路脚步
        [SerializeField] private AudioClip footStepClip;
        [SerializeField, Range(0f, 1f)] private float footStepVolume = 0.7f;
        [SerializeField] private float stepInterval = 0.35f;
        //普通起跳
        [SerializeField] private AudioClip jumpClip;
        [SerializeField, Range(0f, 1f)] private float jumpVolume = 0.65f;
        //落地
        [SerializeField] private AudioClip landClip;
        [SerializeField, Range(0f, 1f)] private float landVolume = 0.6f;
        //空中冲刺
        [SerializeField] private AudioClip dashClip;
        [SerializeField, Range(0f, 1f)] private float dashVolume = 0.6f;
        //蹬墙跳
        [SerializeField] private AudioClip wallJumpClip;
        [SerializeField, Range(0f, 1f)] private float wallJumpVolume = 0.65f;
        //=====新增：拾取音效（集成到玩家身上）=====
        [Header("拾取豆子音效")]
        [SerializeField] private AudioClip coinPickClip;    //小金币pick_coins.wav
        [SerializeField, Range(0f, 1f)] private float coinPickVolume = 0.7f;
        [SerializeField] private AudioClip skillPickClip;   //大豆技能pick_skills.wav
        [SerializeField, Range(0f, 1f)] private float skillPickVolume = 0.7f;

        private AudioSource playerAudio;
        private float footStepTimer;

        public int FacingDirection => facingDirection;
        public bool IsGrounded => isGrounded;
        public bool LandedThisFrame => landedThisFrame;
        public bool JumpedThisFrame => jumpedThisFrame;
        public bool DoubleJumpedThisFrame => doubleJumpedThisFrame;
        public bool WallJumpedThisFrame => wallJumpedThisFrame;
        public bool DashedThisFrame => dashedThisFrame;
        public bool IsDashing => isDashing;
        public bool IsTouchingWall => isTouchingWall;
        public Vector2 Velocity => body != null ? body.velocity : Vector2.zero;
        public float VerticalSpeed => body != null ? body.velocity.y : 0f;
        public float Speed01 => settings != null && settings.maxRunSpeed > 0f
            ? Mathf.Clamp01(Mathf.Abs(Velocity.x) / settings.maxRunSpeed)
            : 0f;

        //=====对外公开两个播放方法，给豆子调用=====
        /// <summary> 捡小金币音效 </summary>
        public void PlayCoinPickSound()
        {
            if (coinPickClip != null)
                playerAudio.PlayOneShot(coinPickClip, coinPickVolume);
        }
        /// <summary> 捡技能大豆音效 </summary>
        public void PlaySkillPickSound()
        {
            if (skillPickClip != null)
                playerAudio.PlayOneShot(skillPickClip, skillPickVolume);
        }

        public void Configure(PlayerMovementSettings movementSettings)
        {
            settings = movementSettings;
            RecalculateJumpValues();
            if (body != null)
            {
                ResetAirJumpCount();
                ResetAirDashCount();
            }
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
            ResetAirJumpCount();
            ResetAirDashCount();

            //自动挂载音源
            playerAudio = GetComponent<AudioSource>();
            if (playerAudio == null)
                playerAudio = gameObject.AddComponent<AudioSource>();
            playerAudio.spatialBlend = 0f;
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
            wallJumpedThisFrame = false;
            dashedThisFrame = false;
            wasGrounded = isGrounded;

            UpdateGroundedState();
            UpdateWallState();
            UpdateTimers();
            TryStartDash();

            //冲刺触发立刻播放音效
            if (dashedThisFrame && dashClip != null)
                playerAudio.PlayOneShot(dashClip, dashVolume);

            if (isDashing)
            {
                HandleDash();
                return;
            }

            HandleJumpInput();
            HandleRun();
            HandleWallSlide();
            HandleGravity();

            //1.走路脚步声
            footStepTimer += Time.fixedDeltaTime;
            float moveXSpeed = Mathf.Abs(body.velocity.x);
            bool isWalking = moveXSpeed > 0.15f;
            if (isWalking && isGrounded && footStepTimer >= stepInterval && footStepClip != null)
            {
                playerAudio.PlayOneShot(footStepClip, footStepVolume);
                footStepTimer = 0f;
            }

            //2.普通起跳音效（原地跳/二段跳）
            if (jumpedThisFrame && jumpClip != null)
                playerAudio.PlayOneShot(jumpClip, jumpVolume);

            //3.蹬墙跳音效
            if (wallJumpedThisFrame && wallJumpClip != null)
                playerAudio.PlayOneShot(wallJumpClip, wallJumpVolume);

            //4.落地音效
            if (landedThisFrame && landClip != null)
                playerAudio.PlayOneShot(landClip, landVolume);
        }

        public void RespawnAtSpawnPoint()
        {
            body.velocity = Vector2.zero;
            transform.position = spawnPosition;
            isDashing = false;
            dashTimer = 0f;
            wallJumpControlLockCounter = 0f;
            ResetAirJumpCount();
            ResetAirDashCount();
        }

        public void SetSpawnPoint(Vector3 position)
        {
            spawnPosition = position;
        }

        public void TeleportTo(Vector3 position, Vector2 velocity)
        {
            transform.position = position;
            body.velocity = velocity;
            isDashing = false;
            dashTimer = 0f;
            wallJumpControlLockCounter = 0f;
            ResetAirJumpCount();
            ResetAirDashCount();
        }

        public bool ConsumeDoubleJumpedThisFrame()
        {
            if (!doubleJumpedThisFrame)
            {
                return false;
            }

            doubleJumpedThisFrame = false;
            return true;
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
                ResetAirJumpCount();
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

            if (wallJumpControlLockCounter > 0f)
            {
                wallJumpControlLockCounter -= step;
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

            if (abilities == null || !abilities.DashUnlocked || isDashing || airDashesRemaining <= 0 || dashCooldownCounter > 0f)
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

            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
        }

        private void ResetAirDashCount()
        {
            airDashesRemaining = settings != null ? Mathf.Max(0, settings.airDashCount) : 0;
        }

        private void ResetAirJumpCount()
        {
            airJumpsRemaining = settings != null ? Mathf.Max(0, settings.airJumpCount) : 0;
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
                return;
            }

            if (jumpBufferCounter > 0f && CanWallJump())
            {
                int jumpDirection = -wallDirection;
                Vector2 velocity = body.velocity;
                velocity.x = jumpDirection * settings.wallJumpHorizontalSpeed;
                velocity.y = jumpVelocity * settings.wallJumpVerticalMultiplier;
                body.velocity = velocity;

                facingDirection = jumpDirection;
                wallJumpControlLockCounter = settings.wallJumpControlLockTime;
                jumpBufferCounter = 0f;
                coyoteCounter = 0f;
                isGrounded = false;
                wallJumpedThisFrame = true;
                return;
            }

            if (jumpBufferCounter > 0f && CanDoubleJump())
            {
                Vector2 velocity = body.velocity;
                velocity.y = jumpVelocity * settings.doubleJumpVelocityMultiplier;
                body.velocity = velocity;

                airJumpsRemaining--;
                jumpBufferCounter = 0f;
                coyoteCounter = 0f;
                isGrounded = false;
                doubleJumpedThisFrame = true;
                jumpedThisFrame = true;
            }

            if (input.ConsumeJumpReleased() && body.velocity.y > 0f)
            {
                Vector2 velocity = body.velocity;
                velocity.y *= settings.jumpCutVelocityMultiplier;
                body.velocity = velocity;
            }
        }

        private bool CanDoubleJump()
        {
            return abilities != null
                && abilities.DoubleJumpUnlocked
                && !isGrounded
                && coyoteCounter <= 0f
                && airJumpsRemaining > 0;
        }

        private bool CanWallJump()
        {
            return abilities != null
                && abilities.WallJumpUnlocked
                && !isGrounded
                && isTouchingWall
                && wallDirection != 0;
        }

        private void HandleRun()
        {
            if (wallJumpControlLockCounter > 0f)
            {
                return;
            }

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

        private void HandleWallSlide()
        {
            if (!CanWallJump() || body.velocity.y >= 0f)
            {
                return;
            }

            Vector2 velocity = body.velocity;
            velocity.y = Mathf.Max(velocity.y, -settings.wallSlideMaxFallSpeed);
            body.velocity = velocity;
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

        private void UpdateWallState()
        {
            isTouchingWall = false;
            wallDirection = 0;

            if (settings == null || bodyCollider == null)
            {
                return;
            }

            Bounds bounds = bodyCollider.bounds;
            Vector2 checkSize = new Vector2(bounds.size.x, bounds.size.y * settings.wallCheckHeight);
            if (CheckWall(bounds.center, checkSize, Vector2.right, 1))
            {
                return;
            }

            CheckWall(bounds.center, checkSize, Vector2.left, -1);
        }

        private bool CheckWall(Vector2 origin, Vector2 checkSize, Vector2 direction, int directionSign)
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(
                origin,
                checkSize,
                0f,
                direction,
                settings.wallCheckDistance,
                settings.groundLayer);

            for (int i = 0; i < hits.Length; i++)
            {
                Collider2D hitCollider = hits[i].collider;
                if (hitCollider == null || hitCollider == bodyCollider || hitCollider.isTrigger)
                {
                    continue;
                }

                isTouchingWall = true;
                wallDirection = directionSign;
                return true;
            }

            return false;
        }
    }
}