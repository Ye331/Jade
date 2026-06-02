using UnityEngine;

namespace Jade.Player
{
    [RequireComponent(typeof(PlayerMotor2D))]
    public class PlayerSpriteAnimator2D : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float idleFramesPerSecond = 2f;
        [SerializeField] private float runFramesPerSecond = 10f;
        [SerializeField] private float runSpeedThreshold = 0.35f;

        private PlayerMotor2D motor;
        private Sprite[] frames;

        public void Configure(SpriteRenderer renderer, Sprite[] animationFrames)
        {
            spriteRenderer = renderer;
            frames = animationFrames;
        }

        private void Awake()
        {
            motor = GetComponent<PlayerMotor2D>();
        }

        private void Update()
        {
            if (spriteRenderer == null || frames == null || frames.Length < 8)
            {
                return;
            }

            spriteRenderer.sprite = frames[GetFrameIndex()];
        }

        private int GetFrameIndex()
        {
            Vector2 velocity = motor.Velocity;
            if (!motor.IsGrounded)
            {
                return velocity.y >= -0.15f ? 6 : 7;
            }

            if (Mathf.Abs(velocity.x) > runSpeedThreshold)
            {
                int runFrame = Mathf.FloorToInt(Time.time * runFramesPerSecond) % 4;
                return 2 + runFrame;
            }

            return Mathf.FloorToInt(Time.time * idleFramesPerSecond) % 2;
        }
    }
}
