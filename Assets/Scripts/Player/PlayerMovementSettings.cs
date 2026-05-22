using UnityEngine;

namespace Jade.Player
{
    [CreateAssetMenu(menuName = "Jade/Player Movement Settings")]
    public class PlayerMovementSettings : ScriptableObject
    {
        [Header("Run")]
        public float maxRunSpeed = 8f;
        public float groundAcceleration = 70f;
        public float groundDeceleration = 90f;
        public float turnDeceleration = 125f;
        public float airAcceleration = 45f;
        public float airDeceleration = 30f;

        [Header("Jump")]
        public float jumpHeight = 4.2f;
        public float timeToJumpApex = 0.46f;
        public float coyoteTime = 0.08f;
        public float jumpBufferTime = 0.12f;
        [Range(0.1f, 1f)] public float jumpCutVelocityMultiplier = 0.45f;

        [Header("Gravity")]
        public float fallGravityMultiplier = 1.55f;
        public float lowJumpGravityMultiplier = 2.05f;
        public float maxFallSpeed = 15f;

        [Header("Dash")]
        public float dashSpeed = 15f;
        public float dashDuration = 0.26f;
        public float dashCooldown = 0.15f;
        public float dashVerticalSpeed = 0f;
        public int airDashCount = 1;

        [Header("Ground Check")]
        public LayerMask groundLayer = ~0;
        public float groundCheckDistance = 0.06f;
        [Range(0.1f, 1f)] public float groundCheckWidth = 0.82f;
    }
}
