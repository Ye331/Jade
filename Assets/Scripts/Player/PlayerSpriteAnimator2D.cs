using UnityEngine;

namespace Jade.Player
{
    [RequireComponent(typeof(PlayerMotor2D))]
    public class PlayerSpriteAnimator2D : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float pixelsPerUnit = 250f;
        [SerializeField] private float idleFramesPerSecond = 2f;
        [SerializeField] private float runFramesPerSecond = 10f;
        [SerializeField] private float runSpeedThreshold = 0.35f;

        private PlayerMotor2D motor;
        private Sprite[] frames;

        public void Configure(SpriteRenderer renderer, Texture2D[] frameTextures, float spritePixelsPerUnit)
        {
            spriteRenderer = renderer;
            pixelsPerUnit = spritePixelsPerUnit;
            BuildSprites(frameTextures);
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

        private void BuildSprites(Texture2D[] frameTextures)
        {
            if (frameTextures == null || frameTextures.Length < 8)
            {
                return;
            }

            frames = new Sprite[frameTextures.Length];
            for (int i = 0; i < frameTextures.Length; i++)
            {
                Texture2D texture = frameTextures[i];
                if (texture == null)
                {
                    frames = null;
                    return;
                }

                Rect rect = new Rect(0f, 0f, texture.width, texture.height);
                frames[i] = Sprite.Create(texture, rect, new Vector2(0.5f, 0.08f), pixelsPerUnit);
                frames[i].name = texture.name + "_RuntimeSprite";
            }
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
