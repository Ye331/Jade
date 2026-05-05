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
        public float timeToJumpApex = 0.42f;
        public float coyoteTime = 0.1f;
        public float jumpBufferTime = 0.12f;
        [Range(0.1f, 1f)] public float jumpCutVelocityMultiplier = 0.45f;

        [Header("Gravity")]
        public float fallGravityMultiplier = 1.75f;
        public float lowJumpGravityMultiplier = 2.25f;
        public float maxFallSpeed = 18f;

        [Header("Ground Check")]
        public LayerMask groundLayer = ~0;
        public float groundCheckDistance = 0.06f;
        [Range(0.1f, 1f)] public float groundCheckWidth = 0.82f;
    }
}
