using Jade.CameraTools;
using UnityEngine;

namespace Jade.Player
{
    [RequireComponent(typeof(PlayerMotor2D))]
    public class PlayerVisualFeedback2D : MonoBehaviour
    {
        [SerializeField] private Transform visualRoot;
        [SerializeField] private ParticleSystem landingDust;
        [SerializeField] private ParticleSystem runDust;
        [SerializeField] private TrailRenderer speedTrail;
        [SerializeField] private float runTiltDegrees = 7f;
        [SerializeField] private float fastSpeedThreshold = 5.5f;
        [SerializeField] private float squashReturnSpeed = 12f;
        [SerializeField] private float landingShakeStrength = 0.06f;
        [SerializeField] private float landingShakeDuration = 0.08f;

        private PlayerMotor2D motor;
        private Vector3 targetScale = Vector3.one;
        private float targetTilt;
        private int visibleFacing = 1;
        private int previousFacing = 1;

        private void Awake()
        {
            motor = GetComponent<PlayerMotor2D>();
        }

        private void Update()
        {
            if (visualRoot == null)
            {
                return;
            }

            UpdateFacing();
            UpdatePose();
            UpdateEffects();
        }

        public void Configure(Transform root, ParticleSystem landParticles, ParticleSystem runParticles, TrailRenderer trail)
        {
            visualRoot = root;
            landingDust = landParticles;
            runDust = runParticles;
            speedTrail = trail;
        }

        private void UpdateFacing()
        {
            visibleFacing = motor.FacingDirection;
            if (visibleFacing != previousFacing && visualRoot != null)
            {
                Vector3 scale = visualRoot.localScale;
                visualRoot.localScale = new Vector3(Mathf.Abs(scale.x) * visibleFacing, scale.y, scale.z);
                previousFacing = visibleFacing;
            }
        }

        private void UpdatePose()
        {
            Vector2 velocity = motor.Velocity;
            float speed01 = Mathf.Clamp01(Mathf.Abs(velocity.x) / fastSpeedThreshold);

            if (motor.LandedThisFrame)
            {
                targetScale = new Vector3(1.18f, 0.82f, 1f);
                targetTilt = 0f;
                PlayLandingFeedback();
            }
            else if (motor.JumpedThisFrame || velocity.y > 1.5f)
            {
                targetScale = new Vector3(0.86f, 1.16f, 1f);
                targetTilt = -visibleFacing * runTiltDegrees * 0.35f;
            }
            else if (!motor.IsGrounded && velocity.y < -1f)
            {
                targetScale = new Vector3(0.96f, 1.04f, 1f);
                targetTilt = visibleFacing * runTiltDegrees * 0.25f;
            }
            else
            {
                targetScale = new Vector3(1f + speed01 * 0.08f, 1f - speed01 * 0.05f, 1f);
                targetTilt = -visibleFacing * runTiltDegrees * speed01;
            }

            Vector3 currentScale = visualRoot.localScale;
            Vector3 signedTargetScale = new Vector3(targetScale.x * visibleFacing, targetScale.y, targetScale.z);
            visualRoot.localScale = Vector3.Lerp(currentScale, signedTargetScale, Time.deltaTime * squashReturnSpeed);

            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetTilt);
            visualRoot.localRotation = Quaternion.Slerp(visualRoot.localRotation, targetRotation, Time.deltaTime * squashReturnSpeed);
        }

        private void UpdateEffects()
        {
            Vector2 velocity = motor.Velocity;
            bool movingFastOnGround = motor.IsGrounded && Mathf.Abs(velocity.x) > fastSpeedThreshold;

            if (speedTrail != null)
            {
                speedTrail.emitting = Mathf.Abs(velocity.x) > fastSpeedThreshold || Mathf.Abs(velocity.y) > 6f;
            }

            if (runDust == null)
            {
                return;
            }

            ParticleSystem.EmissionModule emission = runDust.emission;
            emission.enabled = movingFastOnGround;
        }

        private void PlayLandingFeedback()
        {
            if (landingDust != null)
            {
                landingDust.Play();
            }

            SimpleCameraFollow2D cameraFollow = Camera.main != null ? Camera.main.GetComponent<SimpleCameraFollow2D>() : null;
            if (cameraFollow != null)
            {
                cameraFollow.Shake(landingShakeDuration, landingShakeStrength);
            }
        }
    }
}
