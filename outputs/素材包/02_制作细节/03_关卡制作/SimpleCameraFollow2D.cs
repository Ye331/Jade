using Jade.Player;
using UnityEngine;

namespace Jade.CameraTools
{
    public class SimpleCameraFollow2D : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector2 offset = new Vector2(1.5f, 2.2f);
        [SerializeField] private float lookAheadDistance = 1.25f;
        [SerializeField] private float horizontalSmoothTime = 0.16f;
        [SerializeField] private float verticalSmoothTime = 0.28f;
        [SerializeField] private float minimumX = 0f;
        [SerializeField] private bool useMaximumX;
        [SerializeField] private float maximumX;
        [SerializeField] private bool useYLimits;
        [SerializeField] private Vector2 yLimits = new Vector2(-4f, 4f);
        [SerializeField] private Vector2 deadZone = new Vector2(1.8f, 1.1f);
        [SerializeField] private float lookDownStartX = 100f;
        [SerializeField] private float lookDownOffsetY = -3f;
        [SerializeField] private float lookDownBlendWidth = 8f;

        private float xVelocity;
        private float yVelocity;
        private PlayerMotor2D targetMotor;
        private float shakeTimer;
        private float shakeStrength;

        public void Configure(Transform followTarget)
        {
            target = followTarget;
            targetMotor = target != null ? target.GetComponent<PlayerMotor2D>() : null;
        }

        private void Awake()
        {
            if (target != null)
            {
                targetMotor = target.GetComponent<PlayerMotor2D>();
            }
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            int facing = targetMotor != null ? targetMotor.FacingDirection : 1;
            float lookDownBlend = lookDownBlendWidth > 0f
                ? Mathf.InverseLerp(lookDownStartX, lookDownStartX + lookDownBlendWidth, target.position.x)
                : (target.position.x >= lookDownStartX ? 1f : 0f);
            float verticalOffset = offset.y + lookDownOffsetY * lookDownBlend;
            Vector3 desired = target.position + new Vector3(offset.x + lookAheadDistance * facing, verticalOffset, transform.position.z - target.position.z);
            desired = ApplyDeadZone(desired);

            float x = Mathf.SmoothDamp(transform.position.x, desired.x, ref xVelocity, horizontalSmoothTime);
            float y = Mathf.SmoothDamp(transform.position.y, desired.y, ref yVelocity, verticalSmoothTime);
            x = Mathf.Max(minimumX, x);
            if (useMaximumX)
            {
                x = Mathf.Min(maximumX, x);
            }

            if (useYLimits)
            {
                float minY = Mathf.Min(yLimits.x, yLimits.y);
                float maxY = Mathf.Max(yLimits.x, yLimits.y);
                y = Mathf.Clamp(y, minY, maxY);
            }

            Vector3 shakeOffset = Vector3.zero;
            if (shakeTimer > 0f)
            {
                shakeTimer -= Time.deltaTime;
                shakeOffset = new Vector3(
                    Random.Range(-shakeStrength, shakeStrength),
                    Random.Range(-shakeStrength, shakeStrength),
                    0f);
            }
            else
            {
                shakeStrength = 0f;
            }

            transform.position = new Vector3(x, y, transform.position.z) + shakeOffset;
        }

        private Vector3 ApplyDeadZone(Vector3 desired)
        {
            Vector3 current = transform.position;
            float xDelta = desired.x - current.x;
            float yDelta = desired.y - current.y;

            if (Mathf.Abs(xDelta) <= deadZone.x)
            {
                desired.x = current.x;
            }
            else
            {
                desired.x = current.x + Mathf.Sign(xDelta) * (Mathf.Abs(xDelta) - deadZone.x);
            }

            if (Mathf.Abs(yDelta) <= deadZone.y)
            {
                desired.y = current.y;
            }
            else
            {
                desired.y = current.y + Mathf.Sign(yDelta) * (Mathf.Abs(yDelta) - deadZone.y);
            }

            return desired;
        }

        public void Shake(float duration, float strength)
        {
            shakeTimer = Mathf.Max(shakeTimer, duration);
            shakeStrength = Mathf.Max(shakeStrength, strength);
        }
    }
}
